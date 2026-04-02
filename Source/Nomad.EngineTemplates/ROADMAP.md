# ROADMAP

## What Mature Engine Template Libraries Usually Cover

- Stable template contracts for engine objects, UI elements, scene objects, and
  typed properties.
- Strong versioning rules so generated adapters do not break silently when
  templates change.
- Tooling support that makes template output inspectable and testable.
- Clear ownership between handwritten base types and generated engine-specific
  derivatives.

## Suggested Next Steps

- Document the template model and how each attribute participates in code
  generation.
- Define a versioning strategy for template attributes so engine adapters can
  evolve safely.
- Add examples that show the full flow from template type to generated Godot or
  Unity wrapper.
- Replace the missing module README with package-specific docs.

## Bugfixes And Hardening

- Add a dedicated `Nomad.EngineTemplates.Tests` project, ideally with snapshot
  coverage for generated output expectations.
- Decide whether this package should appear in `NomadSubsystems.json` or live as
  a tooling-only dependency.
- Add validation to catch invalid attribute combinations before generation time.

## Future Additions

- Broader template coverage for physics, audio, networking, and UI widgets.
- Template analyzers that can warn about unsupported engine constructs.
- Interop templates for future Unreal-facing bindings.
