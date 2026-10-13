#!/usr/bin/env python3
"""Opt-in source-structure probe, not a RimWorld/runtime or entry-gate test.

Question: do current AMJE wood trees and comparison fruit trees use disjoint
primary harvestedThingDef paths, making a data-only item swap unsafe?
Owner: Wild Food Foraging; evidence: Docs/Research/TreeNutMechanismAudit-20261014.md.
Retire after the implementation entry choice is accepted and its tests replace
this research-only probe. Never wire to default CI/test discovery.
"""
import argparse
import hashlib
import json
from pathlib import Path
import sys
import xml.etree.ElementTree as ET
from zipfile import ZipFile

MO_XML = '3219596926/1.6/Defs/ThingDefs_Plants/Plants_Cultivated_Farm.xml'
SPECIES = {
    'AMJE': {'AMJ_Tree_Shii': 42, 'AMJ_Tree_Beech': 40},
    'MO': {'DankPyon_Tree_Apple': ('DankPyon_RawApples', 20),
           'DankPyon_Tree_Lemon': ('DankPyon_RawLemons', 12),
           'DankPyon_Tree_Mulberry': ('DankPyon_RawMulberry', 6)},
    'VPE': {'VCE_AppleTree': 'VCE_RawApple',
            'VCE_PeachTree': 'VCE_RawPeach',
            'VCE_CherryTree': 'VCE_RawCherry'},
}


def required(root, names):
    by_name = {node.findtext('defName'): node for node in root.findall('ThingDef')}
    missing = set(names) - by_name.keys()
    if missing:
        raise ValueError('missing Defs: ' + ', '.join(sorted(missing)))
    return by_name


def inspect(environment, mo, vpe):
    roots = [ET.fromstring(xml) for xml in (environment, mo, vpe)]
    env_defs = required(roots[0], SPECIES['AMJE'])
    mo_defs = required(roots[1], SPECIES['MO'])
    vpe_defs = required(roots[2], SPECIES['VPE'])
    observations = {}
    for name, expected_wood in SPECIES['AMJE'].items():
        tree = env_defs[name]
        plant = tree.find('plant')
        if (tree.get('ParentName') not in {'TreeBase', 'DeciduousTreeBase'}
                or plant is None or int(plant.findtext('harvestYield', '-1')) != expected_wood
                or plant.find('harvestedThingDef') is not None
                or plant.find('harvestAfterGrowth') is not None):
            raise ValueError(f'{name}: wood-producing inheritance assumption changed')
        observations[name] = {'parent': tree.get('ParentName'), 'explicit_product': None,
                              'explicit_yield': expected_wood, 'regrowth': None}
    for label, definitions in [('MO', mo_defs), ('VPE', vpe_defs)]:
        for name, expected in SPECIES[label].items():
            tree = definitions[name]
            plant = tree.find('plant')
            if plant is None or tree.get('ParentName') != 'TreeBase':
                raise ValueError(f'{name}: expected a TreeBase fruit tree')
            product = plant.findtext('harvestedThingDef')
            regrowth = float(plant.findtext('harvestAfterGrowth', '0'))
            if (product != (expected[0] if isinstance(expected, tuple) else expected)
                    or regrowth <= 0 or plant.findtext('harvestTag') != 'Standard'):
                raise ValueError(f'{name}: fruit primary yield contract changed')
            if label == 'MO' and int(plant.findtext('harvestYield', '-1')) != expected[1]:
                raise ValueError(f'{name}: MO fruit quantity changed')
            observations[name] = {'parent': 'TreeBase', 'explicit_product': product,
                                  'explicit_yield': float(plant.findtext('harvestYield')),
                                  'regrowth': regrowth}
    return observations


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--environment', required=True, type=Path,
                        help='Current AMJE Defs/ThingDefs_Plants/AMJ_WildPlants.xml')
    parser.add_argument('--mo-archive', required=True, type=Path,
                        help='Author-supplied 3219596926.zip (MO 1.6)')
    parser.add_argument('--vpe', required=True, type=Path,
                        help='VPE 1.6/Defs/ThingDefs_Plants/Plants_Cultivated_Trees.xml')
    args = parser.parse_args()
    env = args.environment.read_bytes()
    with ZipFile(args.mo_archive) as archive:
        mo = archive.read(MO_XML)
    vpe = args.vpe.read_bytes()
    try:
        observations = inspect(env, mo, vpe)
    except (ET.ParseError, ValueError, TypeError) as exc:
        print(f'SOURCE DRIFT: {exc}', file=sys.stderr)
        return 1
    print(json.dumps({'evidence': 'source structure only; NOT native gameplay',
                      'sha256': {'AMJE': hashlib.sha256(env).hexdigest(),
                                 'MO XML': hashlib.sha256(mo).hexdigest(),
                                 'VPE': hashlib.sha256(vpe).hexdigest()},
                      'observations': observations}, ensure_ascii=False, indent=2))
    return 0


if __name__ == '__main__':
    sys.exit(main())
