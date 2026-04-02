# ROADMAP

## What Mature Core Packages Usually Cover

- Stable primitives for lifecycle, dependency injection, threading, guards,
  collections, memory, and shared value objects.
- Tight API ownership so higher-level subsystem concepts do not leak back into
  the foundation package without a strong reason.
- Benchmarks for hot-path containers, pools, and lock-free collections in
  addition to normal unit tests.
- Clear contracts for engine abstractions such as scene objects, input events,
  file wrappers, and service lookup.

## Suggested Next Steps

- Split and document the internal domains inside `Nomad.Core` so consumers can
  tell which namespaces are true foundation APIs versus temporary holding areas.
- Add benchmark coverage for `LockFreeQueue`, `LockFreeRingBuffer`,
  `BasicObjectPool`, and buffer handle types.
- Add explicit guidance for when to use global service access versus scoped
  service resolution.
- Write a real module README for `Nomad.Core` instead of the current generic
  framework copy.

## Bugfixes And Hardening

- Audit namespace boundaries to keep subsystem-specific logic from collecting in
  `Nomad.Core` simply because it has no other home yet.
- Add package-level smoke tests for service registry disposal, thread-guard
  failures, and memory handle lifetime mistakes.
- Document thread-safety guarantees for the service registry and shared memory
  utilities.

## Future Additions

- Custom container implementations with allocator-aware variants for high churn
  runtime paths.
- A job system or task scheduler abstraction for host engines that need stricter
  frame control.
- Performance instrumentation hooks that other subsystems can plug into without
  taking a logger dependency.
