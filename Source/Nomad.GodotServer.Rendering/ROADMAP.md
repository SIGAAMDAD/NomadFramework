# ROADMAP

## What Mature Server-Side Rendering State Packages Usually Cover

- Authoritative state models for animations, sprites, transforms, and visibility
  that can be consumed by clients or debugging tools.
- Clear separation between render-state generation and actual client rendering.
- Efficient delta generation so server snapshots stay compact.
- Debugging views that help multiplayer developers inspect what the server thinks
  should be visible.

## Suggested Next Steps

- Document the intended role of this package because it currently has no
  dedicated README and its purpose is not obvious from the repo root.
- Clarify how this package relates to headless servers, client prediction, and
  `Nomad.EngineUtils.Godot`.
- Add sample DTO or snapshot flows showing how render entities leave the server.
- Decide whether this package is experimental or ready to appear in wider
  metadata.

## Bugfixes And Hardening

- Add a dedicated `Nomad.GodotServer.Rendering.Tests` project for entity state
  generation, animation updates, and event ordering.
- Validate serialization and equality behavior for the private DTO types.
- Add contract tests around bootstrap timing and service availability.

## Future Additions

- Delta-compressed state streaming.
- Visibility and culling helpers.
- Debug overlays that visualize server-side render state in development builds.
