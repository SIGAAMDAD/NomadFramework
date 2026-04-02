# ROADMAP

## What Mature Resource Caches Usually Cover

- Async loading, de-duplication, eviction policies, and explicit memory budgets.
- Load progress, load failure, and unload notifications that consumers can trust.
- Support for different residency strategies such as keep-hot, stream-on-demand,
  and pin-until-release.
- Visibility into hit rate, churn, and dependency-driven unloading.

## Suggested Next Steps

- Add budget-aware caching so eviction is driven by real memory policy instead
  of simple count-based heuristics.
- Define how cache entries should represent dependencies between banks,
  textures, scenes, and engine-specific wrappers.
- Add stronger examples that show how engine adapters should plug in custom
  loaders.
- Replace the generic package README with module-specific cache docs.

## Bugfixes And Hardening

- Grow test coverage beyond the current base cache test so load failure,
  eviction, and progress events are all locked down.
- Document thread affinity for load callbacks and unload events.
- Add regression tests for duplicate concurrent loads resolving to one shared
  cache entry.

## Future Additions

- Streaming and prefetch queues for large content sets.
- Cache heatmaps and residency diagnostics for performance tuning.
- Background warm-up plans that cooperate with engine loading screens.
