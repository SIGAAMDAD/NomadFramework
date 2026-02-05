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
- **Event System** — decoupled, high‑performance event dispatching  
- **CVar System** — idTech‑style configuration variables  
- **Developer Console** — runtime command execution and debugging  
- **Custom ECS** — lightweight, cache‑friendly entity‑component system  
- **Input System** — unified input abstraction across engines  
- **Logging System** — structured, configurable logging  

### Data & Persistence
- **Nomad.Save** — deterministic serialization, versioning, and state restoration  
- **Nomad.FileSystem - Virtual File System (VFS)** — Quake 3‑style layered filesystem  
- **Nomad.ResourceCache** — unified asset access across engines  

### Audio & Media
- **Nomad.Audio** - an abstraction layer over audio pipelines including OpenAL, Godot, and FMOD
- **Nomad.Audio.FMOD** — high‑quality audio pipeline support  

### Networking
- **Matchmaking System** — lobby creation, discovery, and session management  
- **Nomad.OnlineServices.Steam** — engine‑agnostic Steamworks.NET abstraction layer
- **Nomad.OnlineServices.GOG** - engine-agnostic G.O.G. abstraction layer

### Gameplay Systems
- **Quest API** — data‑driven quest and objective logic  
- **Nomad.Events** — reusable gameplay event definitions  

### Extensibility
- Fully modular subsystem design, you can pick and choose which ones you want without the entire framework
- Additional modules planned for future releases
- Designed for long‑term maintainability and cross‑project reuse

---

## Philosophy

NomadFramework is built around a simple principle:

**Never reinvent the wheel twice.**

After years of solo development and maintaining long‑lived codebases, the framework was designed to capture the universal patterns that appear in every engine:

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

Planned future modules include:

- Additional engine bindings  
- Advanced networking features  
- Physics abstraction layer  
- UI subsystem  
- Scripting integration  
- Asset pipeline tools  

---

## Contributing

Contributions are welcome.  
Please follow the formatting rules in `FORMATTING.md` and ensure all code is documented, tested, and consistent with the project’s architectural principles.

---

## Contact

For licensing inquiries, contributions, or questions, please reach out to the project maintainer.