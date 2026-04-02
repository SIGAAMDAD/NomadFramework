# ROADMAP

## What Mature Audio Abstraction Layers Usually Cover

- Stable interfaces for devices, emitters, listeners, groups, music, and
  parameter control.
- Explicit lifetime rules so backend implementations can share common calling
  semantics.
- A minimal feature set that all backends support plus a capability model for
  backend-specific extras.
- Examples that show how higher-level gameplay code should stay backend-neutral.

## Suggested Next Steps

- Define backend capability flags so hosts can branch intentionally when a
  backend cannot support a feature.
- Add a reference backend or thin fake implementation to exercise the contracts
  without FMOD.
- Clarify the ownership model between emitters, channels, groups, and music
  services.
- Replace the generic package README with backend-agnostic audio integration
  docs.

## Bugfixes And Hardening

- Add a dedicated `Nomad.Audio.Tests` project because the package currently has
  interface surface but no direct contract tests.
- Add API-level validation tests for null listeners, invalid handles, and
  lifecycle misuse by backend implementations.
- Document which abstractions are required for all backends versus optional.

## Future Additions

- Additional backends such as OpenAL or an engine-native Godot path.
- Audio debugging overlays and profiling hooks.
- Gameplay-facing helpers for ducking, priorities, and music state machines.
