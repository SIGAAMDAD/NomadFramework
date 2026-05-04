. "$PSScriptRoot/common.ps1"

Write-NomadSection "Build"

Assert-NomadPathExists $Script:SolutionPath "Solution"

Invoke-NomadCommand $Script:DotNet @(
	"build",
	$Script:SolutionPath,
	"--configuration",
	"Debug",
	"--no-restore",
	"-p:RunAnalyzers=true",
	"-p:ContinuousIntegrationBuild=true"
)