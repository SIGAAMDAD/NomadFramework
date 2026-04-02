# ROADMAP

This file is the framework-level roadmap index for `Source/NomadFramework`.
Each package under `Source/NomadFramework/Source` now owns its own `ROADMAP.md`
so subsystem planning can stay close to the code it describes.

## Common Expectations For Every Subsystem

- A clear bootstrapper and lifecycle contract so hosts know when the subsystem is
  safe to initialize, update, and tear down.
- A small, intentional public API surface that is separated cleanly from private
  implementation details and engine-specific adapters.
- A dedicated, module-specific README with quick start steps, dependencies,
  engine compatibility notes, and a minimal working example.
- A matching test project that covers happy paths, edge cases, failure handling,
  and serialization or threading behavior where relevant.
- Structured diagnostics through logging, events, counters, and meaningful
  exceptions rather than silent fallback behavior.
- Stable package metadata and repo manifests so every shippable package is
  represented consistently in `NomadSubsystems.json`, NuGet metadata, docs, and
  CI.

## Framework-Wide Priority Work

- Clean up subsystem metadata. `NomadSubsystems.json` currently duplicates
  `Nomad.Events`, contains a typo in the `Nomad.FileSystem` description, and
  does not describe several packages that exist in `Source/`.
- Replace generic package READMEs with module-specific documentation. Most
  current package README files are copies of the root README rather than docs
  for the package they live beside.
- Close test coverage gaps. `Nomad.Audio`, `Nomad.Audio.FMOD`, `Nomad.Console`,
  `Nomad.EngineTemplates`, `Nomad.EngineUtils.Godot`,
  `Nomad.EngineUtils.Settings`, `Nomad.EngineUtils.Unity`,
  `Nomad.GodotServer.Rendering`, `Nomad.Logger`, `Nomad.SourceGenerators`, and
  `Nomad.Steam.VoiceChat` do not currently have dedicated test projects.
- Standardize release hygiene. Decide which generated docs, build artifacts, and
  engine export outputs should be committed, indexed, or excluded from package
  discovery.
- Add end-to-end sample hosts. The framework would benefit from a small Godot
  sample, a headless sample, and a Unity sample that exercise the same core
  packages.

## Subsystem Roadmaps

### Foundation

- [Nomad.Core](./Source/Nomad.Core/ROADMAP.md)
- [Nomad.CVars](./Source/Nomad.CVars/ROADMAP.md)
- [Nomad.Events](./Source/Nomad.Events/ROADMAP.md)
- [Nomad.Logger](./Source/Nomad.Logger/ROADMAP.md)
- [Nomad.Console](./Source/Nomad.Console/ROADMAP.md)

### Data And Runtime Services

- [Nomad.FileSystem](./Source/Nomad.FileSystem/ROADMAP.md)
- [Nomad.Save](./Source/Nomad.Save/ROADMAP.md)
- [Nomad.ResourceCache](./Source/Nomad.ResourceCache/ROADMAP.md)
- [Nomad.Input](./Source/Nomad.Input/ROADMAP.md)

### Audio And Online

- [Nomad.Audio](./Source/Nomad.Audio/ROADMAP.md)
- [Nomad.Audio.FMOD](./Source/Nomad.Audio.FMOD/ROADMAP.md)
- [Nomad.OnlineServices.Steam](./Source/Nomad.OnlineServices.Steam/ROADMAP.md)
- [Nomad.Steam.VoiceChat](./Source/Nomad.Steam.VoiceChat/ROADMAP.md)

### Engine Adapters And Tooling

- [Nomad.EngineUtils.Godot](./Source/Nomad.EngineUtils.Godot/ROADMAP.md)
- [Nomad.EngineUtils.Settings](./Source/Nomad.EngineUtils.Settings/ROADMAP.md)
- [Nomad.EngineUtils.Unity](./Source/Nomad.EngineUtils.Unity/ROADMAP.md)
- [Nomad.GodotServer.Rendering](./Source/Nomad.GodotServer.Rendering/ROADMAP.md)
- [Nomad.EngineTemplates](./Source/Nomad.EngineTemplates/ROADMAP.md)
- [Nomad.SourceGenerators](./Source/Nomad.SourceGenerators/ROADMAP.md)

## Long-Range Additions

- Epic Games Store and GOG online service adapters.
- Unreal-facing bindings that feel first-party rather than bolt-on.
- A lightweight ECS package if that remains part of the long-term framework
  vision.
- More engine bridges beyond Godot and Unity once the shared contracts settle.
