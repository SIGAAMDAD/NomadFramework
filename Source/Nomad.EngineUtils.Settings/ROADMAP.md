# ROADMAP

## What Mature Settings Layers Usually Cover

- Strongly typed display, graphics, and audio settings objects.
- Persistence and profile switching layered on top of CVars or config storage.
- Validation and compatibility checks when settings are applied live.
- UI-friendly metadata so settings menus do not have to hard-code every range,
  label, or preset.

## Suggested Next Steps

- Clarify whether this package is a true subsystem or a support package, then
  reflect that decision in `NomadSubsystems.json`.
- Add preset support for common graphics, accessibility, and audio bundles.
- Define how settings changes flow into engine adapters and save or config
  storage.
- Replace the current generic README with a settings-focused guide.

## Bugfixes And Hardening

- Add a dedicated `Nomad.EngineUtils.Settings.Tests` project for validation,
  persistence, and cvar integration behavior.
- Add compatibility checks for unsupported display modes or out-of-range config
  values before those settings reach the host engine.
- Make sure service naming stays clear between the generic `SettingsService` and
  the more specialized audio or display services.

## Future Additions

- Settings diff and rollback support.
- Per-platform override layers.
- Auto-generated settings UI from cvar and config metadata.
