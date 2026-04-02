# ROADMAP

## What Mature FMOD Integration Layers Usually Cover

- Reliable bank loading, event playback, listener management, and channel or bus
  control.
- Clear separation between backend-neutral audio contracts and FMOD-specific
  optimizations.
- Robust diagnostics for missing banks, invalid GUIDs, and platform-specific
  native library issues.
- Testability through wrappers or seams around the FMOD runtime.

## Suggested Next Steps

- Add a fake or adapter seam around FMOD calls so the integration can be tested
  without native runtime dependencies.
- Document bank loading strategy and preload expectations for gameplay teams.
- Add profiling hooks for active voices, bank residency, and event allocation
  churn.
- Replace the generic package README with FMOD-specific setup and troubleshooting
  docs.

## Bugfixes And Hardening

- Refactor `Private/Repositories/FMODChannelRepository.cs`, which already calls
  out SOLID and performance concerns inside the file.
- Replace the placeholder listener error note in
  `Private/Services/FMODListenerService.cs` with a specific exception type or
  recovery path.
- Confirm the architecture mapping TODO in `Plugins/FMOD/fmod_calls.cs` for
  ARMv6 or remove the ambiguity.
- Add a dedicated `Nomad.Audio.FMOD.Tests` project for bank loading, channel
  ownership, and native-error translation behavior.

## Future Additions

- Live bank reload during development.
- Snapshot and parameter automation helpers.
- FMOD profiler export or in-engine visualization.
