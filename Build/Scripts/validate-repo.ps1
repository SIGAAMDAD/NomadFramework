dotnet run -f net10.0 --project Tools/Nomad.PackageValidator -- --repo-root . --strict
dotnet run -f net10.0 --project Tools/Nomad.TestProjectValidator -- --repo-root . --strict
dotnet run -f net10.0 --project Tools/Nomad.HeaderValidator -- --repo-root . --include-tests
dotnet run -f net10.0 --project Tools/Nomad.CompatibilityMatrixGenerator -- --repo-root . --validate-only