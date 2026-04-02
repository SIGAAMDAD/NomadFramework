# ROADMAP

## What Mature Voice Chat Layers Usually Cover

- Capture, encode, decode, transport integration, and per-user playback control.
- Mute, push-to-talk, voice activity detection, and quality settings.
- Runtime diagnostics for packet loss, decode failures, and user state.
- Clean separation between transport concerns and audio playback concerns.

## Suggested Next Steps

- Define how this package layers on top of `Nomad.OnlineServices.Steam` and
  `Nomad.Audio` so ownership is unambiguous.
- Expose voice chat settings such as push-to-talk, input gain, and output gain
  through CVars or a small config object.
- Add developer-facing docs for the expected networking and audio plumbing.
- Replace the missing module README with a package-specific guide.

## Bugfixes And Hardening

- Add a dedicated `Nomad.Steam.VoiceChat.Tests` project for encode or decode
  correctness, user state transitions, and error handling.
- Add explicit handling for corrupt or partial voice payloads.
- Document the lifecycle rules for `SteamVoiceService` and `VoiceUserData`.

## Future Additions

- Voice activity detection and automatic gain support.
- Distance attenuation and spatial routing when paired with world audio.
- Moderation hooks such as per-user blocks, recording, or transcription.
