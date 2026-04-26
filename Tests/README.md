# NomadFramework Tests

The test suite is organized first by framework module, then by the feature area
under test. Test files keep their original namespaces and fixture names so NUnit
discovery remains stable while the filesystem layout is easier to scan.

## Layout

- `Mocks/`: shared test doubles.
- `Nomad.Core.Tests/`: guards, collections, memory, rendering, services,
  serialization, value objects, windowing, and bootstrap coverage.
- `Nomad.CVars.Tests/`: CVar conversion, loading, metadata, validation, value
  objects, system, and bootstrap coverage.
- `Nomad.Events.Tests/`: event types, flags, queueing, registry, subscriptions,
  identity, stress, and bootstrap coverage.
- `Nomad.FileSystem.Tests/`: file streams, memory streams, services, search,
  and bootstrap coverage.
- `Nomad.Input.Tests/`: bindings, dispatch, loading, rebinding, state, system,
  fixtures, support helpers, and performance coverage.
- `Nomad.OnlineServices.Steam.Tests/`: Steam achievements, cloud, and lobby
  coverage.
- `Nomad.ResourceCache.Tests/`: cache coverage.
- `Nomad.Save.Tests/`: save sections, corruption, metadata, data providers,
  error handling, regression, versioning, and bootstrap coverage.

## Categories

Fixtures are labeled with:

- Module categories such as `Nomad.Core`, `Nomad.Events`, and `Nomad.Save`.
- Feature categories derived from their folder, such as `Guards`,
  `Streams.Memory`, `Sections.Reading`, or `Lobbies`.
- Test-kind categories such as `Unit`, `Integration`, `Performance`, `Stress`,
  and `Regression`.

Examples:

```powershell
dotnet test --filter "TestCategory=Nomad.Events"
dotnet test --filter "TestCategory=Performance"
dotnet test --filter "TestCategory!=Steam"
```
