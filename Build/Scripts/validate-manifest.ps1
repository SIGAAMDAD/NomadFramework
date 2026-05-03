$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Resolve-Path (Join-Path $ScriptDir '../..')
dotnet run -f net10.0 --project (Join-Path $RepoRoot 'Tools/Nomad.ManifestValidator/Nomad.ManifestValidator.csproj') -- --repo-root $RepoRoot @args
