# Temporary research tests

Opt-in only; no active experiments. Never include in default CI or normal test discovery. Record question, owner, evidence destination and retirement condition before adding an experiment.

Current opt-in probe: `audit_tree_harvest_sources.py` compares existing AMJE Shii/Beech XML, the author-provided MO 1.6 archive and VPE 1.6 fruit-tree XML. Research question: is a data-only fruit-primary swap safe for original wood-primary trees? Evidence destination: `Docs/Research/TreeNutMechanismAudit-20261014.md`. Retirement: after the approved implementation entry decision and real producer/runtime checks supersede this source-only comparison. Run manually with `--environment`, `--mo-archive` and `--vpe`; it is intentionally not part of standard CI.
