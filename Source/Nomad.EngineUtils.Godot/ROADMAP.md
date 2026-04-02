# ROADMAP

## What Mature Godot Adapter Layers Usually Cover

- Engine-native wrappers for input, scenes, resources, logging, and window or
  display behavior.
- Conversions between engine types and framework value objects with predictable
  allocation behavior.
- Clear bootstrap and shutdown guidance for editor, play mode, and exported
  builds.
- Enough samples that Godot users can adopt the framework without reading the
  entire source tree.

## Suggested Next Steps

- Document how this package should work with `Nomad.EngineTemplates` and
  `Nomad.SourceGenerators`.
- Add smoke tests or sample scenes that exercise the Godot console, input pump,
  and scene wrappers.
- Expand coverage of engine-specific services such as localization, display, and
  scene loading.
- Replace the generic package README with real Godot integration docs.

## Bugfixes And Hardening

- Resolve the zero-allocation TODO in `Public/GodotGameObject.cs` or document
  the accepted allocation cost.
- Add regression coverage for type conversions, resource loading, and game
  object wrapping behavior.
- Verify editor-only and export-only code paths so the package behaves
  consistently in both environments.

## Future Additions

- Better Godot editor tooling and setup helpers.
- Scene conversion or template generation workflows.
- Tighter integration with the separate `addons/NomadFramework` editor plugin if
  that remains part of the overall plan.
