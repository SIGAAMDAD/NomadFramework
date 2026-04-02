# ROADMAP

## What Mature Runtime Console Systems Usually Cover

- Command registration, parsing, help text, history, and argument validation.
- Integration with CVars and logging so the console can inspect and mutate live
  runtime state.
- Auto-complete, suggestions, aliases, and machine-readable command metadata.
- Permission and visibility controls for shipping versus debug builds.

## Suggested Next Steps

- Define a formal command metadata model so help, auto-complete, and docs all
  read from the same contract.
- Add richer parsing for quoted values, enums, optional arguments, and command
  overloads.
- Add better integration guidance for engine-hosted consoles in Godot and
  Unity.
- Replace the missing module README with a package-specific guide.

## Bugfixes And Hardening

- Add a dedicated `Nomad.Console.Tests` project for parser edge cases, history
  behavior, cache invalidation, and command failure handling.
- Verify that command discovery and caching stay correct if commands are added
  after bootstrap.
- Add tests for interaction between console commands and cvar mutation events.

## Future Additions

- Remote console access for dedicated servers.
- Command permissions by environment or build type.
- A browser-based or in-game console frontend.
