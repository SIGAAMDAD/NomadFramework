$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Resolve-Path (Join-Path $ScriptDir '../..')
dotnet run -f net10.0 --project (Join-Path $RepoRoot 'Tools/Nomad.ApiSurfaceValidator/Nomad.ApiSurfaceValidator.csproj') -- --repo-root $RepoRoot @args
