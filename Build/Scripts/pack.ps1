. "$PSScriptRoot/common.ps1"

Write-NomadSection "Pack"

Assert-NomadPathExists $Script:SolutionPath "Solution"

if ( Test-Path $Script:NuGetOutputDirectory ) {
	Remove-Item $Script:NuGetOutputDirectory -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $Script:NuGetOutputDirectory | Out-Null

Invoke-NomadCommand $Script:DotNet @(
	"pack",
	$Script:SolutionPath,
	"--configuration",
	$Script:Configuration,
	"--no-build",
	"--output",
	$Script:NuGetOutputDirectory,
	"-p:GeneratePackages=true",
	"-p:ContinuousIntegrationBuild=true"
)