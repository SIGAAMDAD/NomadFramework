# NomadFramework

NomadFramework is an engine‑agnostic, C#‑based game backend designed to eliminate wheel‑reinvention across projects. It provides a unified runtime layer that can be embedded into Unity, Godot, Unreal (via C# bindings), or custom engines. The framework is modular, subsystem‑complete, and built on universal engine principles found in idTech, Doom, Unreal, and Godot.

NomadFramework is licensed under **MPL‑2.0**, with an optional **commercial license** (1% royalty after store fees).

---

## Features

NomadFramework includes a comprehensive suite of subsystems intended to serve as the foundation for any game project:

### Core Architecture
- Engine‑agnostic design  
- C# version‑agnostic  
- Modular subsystem architecture  
- Clean, documented API surface  
- Optional commercial licensing  

### Runtime Subsystems
- **Nomad.Events** — decoupled, high‑performance event dispatching
- **Nomad.CVars** — idTech‑style configuration variables
- **Nomad.Console** — runtime command execution and debugging
- **Nomad.ECS** — lightweight, cache‑friendly entity‑component system
- **Nomad.Input** — unified input abstraction across engines, multiple binds, coyote time, etc.
- **Nomad.Logging** — structured, configurable logging with custom sink suppoort

### Data & Persistence
- **Nomad.Save** — deterministic serialization, versioning, and state restoration  
- **Nomad.FileSystem - Virtual File System (VFS)** — Quake 3‑style layered filesystem  
- **Nomad.ResourceCache** — unified asset access across engines  

### Audio & Media
- **Nomad.Audio** - an abstraction layer over audio pipelines including OpenAL, Godot, and FMOD
- **Nomad.Audio.FMOD** — high‑quality audio pipeline support  

### Networking
- **Nomad.OnlineServices.Steam** — engine‑agnostic Steamworks.NET abstraction layer

### Gameplay Systems
- **Nomad.Quests** — data‑driven quest and objective logic

### Extensibility
- Fully modular subsystem design, you can pick and choose which ones you want without the entire framework
- Additional modules planned for future releases
- Designed for long‑term maintainability and cross‑project reuse

---

## Philosophy

NomadFramework is built around a simple principle:

**Never reinvent the wheel twice.**
And truly
**Write once, run everywhere**

This framework was designed to capture the universal patterns that appear in every engine:

- CVars
- Consoles
- Event Buses
- ECS
- Save Systems (the good ones)
- VFS layers
- input abstractions
- logging
- networking
- audio integration

These patterns are stable across decades of engine design. NomadFramework unifies them into a portable backend that can outlive any single engine or project.

---

## Licensing

NomadFramework is distributed under:

- **MPL‑2.0** for open‑source use
- **Commercial License** for closed‑source or commercial projects
  - 1% royalty after store fees

Contact the author for commercial licensing details.

---

## Formatting & Code Standards

This project follows a strict formatting and documentation standard to ensure long‑term maintainability.
See [`FORMATTING.md`](./FORMATTING.md) for full details.

---

## Roadmap

Planned future modules & features are listed in great depth within the [`ROADMAD.md`](./ROADMAP.md) document.

---

## Contributing

Contributions are welcome.  
Please follow the formatting rules in `FORMATTING.md` and ensure all code is documented, tested, and consistent with the project’s architectural principles.

---

## Contact

For licensing inquiries, contributions, or questions, please reach out to the project maintainer.
