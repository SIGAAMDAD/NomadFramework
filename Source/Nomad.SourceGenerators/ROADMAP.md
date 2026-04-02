# ROADMAP

## What Mature Source Generator Packages Usually Cover

- Deterministic generation, clear diagnostics, and safe incremental behavior.
- Snapshot tests that lock down output shape and error messages.
- Strong input validation so malformed attributes fail with actionable feedback.
- Tooling docs that explain when generation runs and where output should be
  expected.

## Suggested Next Steps

- Add explicit diagnostics for unsupported template shapes and missing required
  attributes.
- Document how `Nomad.SourceGenerators` and `Nomad.EngineTemplates` divide
  responsibility.
- Add a small sample project that demonstrates generation end to end.
- Replace the missing module README with source-generator-specific docs.

## Bugfixes And Hardening

- Add a dedicated `Nomad.SourceGenerators.Tests` project with snapshot tests.
- Verify deterministic output ordering so generated files do not churn between
  builds.
- Review whether the generator should move toward Roslyn incremental patterns if
  generation scope keeps growing.

## Future Additions

- A linting layer for template definitions.
- Code fixes or analyzer suggestions that help authors repair invalid templates.
- Generator performance metrics surfaced during CI.
