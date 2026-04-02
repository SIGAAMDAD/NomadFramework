# ROADMAP

## What Mature Save Systems Usually Cover

- Deterministic serialization, version headers, checksums, and corruption
  detection.
- Slot management, backups, migration, and partial recovery after failed writes.
- Strong schema ownership so gameplay teams know how data evolves over time.
- Tooling that can inspect, diff, and validate save files outside the game.

## Suggested Next Steps

- Turn migration and recovery into first-class workflows with docs, extension
  points, and sample upgrade paths.
- Add save manifest support so slots can be enumerated without opening every
  file in full.
- Add a public stance on compression, encryption, and cloud-safe metadata.
- Replace the generic package README with save-format and migration guidance.

## Bugfixes And Hardening

- Expand corruption tests to cover backup restoration, interrupted writes, and
  mixed-version section recovery.
- Make the debug logging path easier to reason about by documenting when the
  save cvars must already exist versus when safe defaults are used.
- Add regression tests around duplicate sections, duplicate fields, and schema
  mismatches across game versions.

## Future Additions

- Save diff and inspection tooling for developers and modders.
- Optional compression and encryption policies per section or per slot.
- Cloud save adapters layered on top of the local provider model.
