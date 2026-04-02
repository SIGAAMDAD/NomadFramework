# ROADMAP

## What Mature Input Systems Usually Cover

- Action-based input, multiple bindings per action, composite bindings, and
  device abstraction.
- Device hot-swap, rebinding, deadzone and response curve tuning, and conflict
  detection.
- Predictable action phases and frame-accurate dispatch for gameplay code.
- Serialization and migration for default binds and user overrides.

## Suggested Next Steps

- Add formal rebind workflows and UI-facing metadata so game code does not have
  to reverse-engineer binding definitions.
- Add better device hot-plug and active-device tracking across keyboard, mouse,
  and gamepad paths.
- Clarify the long-term split between public constants and private constants so
  the two files do not drift semantically.
- Replace the generic package README with usage docs centered on actions,
  bindings, and events.

## Bugfixes And Hardening

- Expand tests for conflict resolution, composite binding collisions, and
  default bind migration.
- Add integration coverage for gamepad deadzones, response curves, and mixed
  device input in the same frame.
- Verify persistence and round-tripping of user-customized binding data.

## Future Additions

- Touch and mobile input adapters.
- Input recording and deterministic replay.
- Steam Input or platform-native glyph integration.
