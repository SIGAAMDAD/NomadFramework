# ROADMAP

## What Mature CVar Systems Usually Cover

- Strongly typed reads and writes, validation, defaults, grouping, and
  discoverability.
- Persistence across config files, command line overrides, and runtime edits.
- Event hooks so settings changes can update live systems without polling.
- Tooling for exporting schemas, generating docs, and surfacing safe ranges to
  UI code.

## Suggested Next Steps

- Add schema export so UIs, docs, and test fixtures can consume a single source
  of truth for names, ranges, defaults, and descriptions.
- Support layered configuration sources beyond the current INI flow, such as
  user overrides, profile presets, and environment-specific values.
- Add command-friendly metadata like aliases, hidden flags, and deprecated-name
  redirects.
- Replace the generic package README with one focused on registration,
  validation, and persistence behavior.

## Bugfixes And Hardening

- Expand tests around malformed INI input, duplicate keys, and backward
  compatibility for renamed CVars.
- Verify that change events cannot recurse into unstable update loops when one
  cvar setter mutates another.
- Add a compatibility checklist for subsystems that register CVars at bootstrap
  time.

## Future Additions

- Remote tuning support for live gameplay balancing.
- Setting profiles that can switch groups of CVars in one operation.
- Generated settings menus that derive controls from CVar metadata.
