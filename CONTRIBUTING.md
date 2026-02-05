# Contributing to NomadFramework

Thank you for your interest in contributing to NomadFramework.  
This project follows strict architectural, formatting, and documentation standards to ensure long‑term maintainability and consistency across all modules.  
Please read this document carefully before submitting any contributions.

---

## Code of Conduct

All contributors are expected to maintain a professional and respectful environment.  
Technical disagreements are normal; personal attacks or unprofessional behavior are not tolerated.

---

## Development Philosophy

NomadFramework is a subsystem‑complete, engine‑agnostic backend intended for long‑term reuse across multiple engines and projects.  
Contributions must align with the following principles:

- **Portability** — Code must remain engine‑agnostic unless inside a dedicated engine‑binding module.
- **Modularity** — Subsystems must be isolated, replaceable, and self‑contained.
- **Predictability** — APIs should be explicit, documented, and stable.
- **Performance** — Avoid unnecessary allocations, reflection, or hidden abstractions.
- **Maintainability** — Code must be readable, consistent, and follow the formatting rules.

If a contribution introduces complexity, it must justify that complexity with clear architectural benefit.

---

## Repository Structure

The repository is organized into modules. Each module:

- resides in its own directory  
- contains its own documentation  
- must not depend on engine‑specific code  
- must follow the formatting rules in `FORMATTING.md`  

Engine bindings (Unity, Godot, Unreal, etc.) live in separate integration layers.

---

## Module Dependency Rules

To maintain portability and prevent dependency creep:

- Modules **may only depend on**:
  - `Core` (interfaces, enums, shared primitives)
  - `Events`
  - `Logging`
  - `CVars`

- Additional dependencies must be justified and approved through discussion.

- Modules must **never** depend on each other unless the dependency is:
  - absolutely required,
  - documented,
  - and architecturally sound.

Circular dependencies are strictly prohibited and will be rejected.

---

## How to Contribute

### 1. Fork the Repository
Create a fork of the repository on your own GitHub account.

### 2. Create a Feature Branch
Use a descriptive branch name:
