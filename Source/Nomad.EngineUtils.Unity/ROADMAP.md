# ROADMAP

## What Mature Unity Adapter Layers Usually Cover

- Bridges for logging, display, console, input, and engine object wrappers.
- Clear support for edit mode, play mode, and packaged builds.
- Compatibility with Unity package distribution and common project structures.
- Enough samples that Unity consumers know what initialization order is
  expected.

## Suggested Next Steps

- Define whether this package targets legacy Unity APIs, the newer input stack,
  or both.
- Add example bootstrap code for Unity scenes and package-style installation.
- Document how this package should consume `Nomad.EngineTemplates` output.
- Replace the current generic README with Unity-specific setup docs.

## Bugfixes And Hardening

- Add a dedicated `Nomad.EngineUtils.Unity.Tests` project for console, debug
  sink, input pump, and type conversion behavior.
- Decide whether this package belongs in `NomadSubsystems.json` and package
  metadata alongside the shipping runtime subsystems.
- Verify API assumptions against Unity domain reload behavior and scene reloads.

## Future Additions

- UPM packaging and editor tooling.
- Better integration with Unity's input, localization, and addressable systems.
- Sample projects that mirror the Godot adapter feature set.
