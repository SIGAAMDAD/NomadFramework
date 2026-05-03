. "$PSScriptRoot/common.ps1"

Write-NomadSection "Coverage"

Assert-NomadPathExists $Script:TestProjectPath "Test project"
Assert-NomadPathExists $Script:CoverageSettingsPath "Coverage settings"

$filter = if ( $env:NOMAD_COVERAGE_TEST_FILTER ) {
	$env:NOMAD_COVERAGE_TEST_FILTER
} else {
	"""Category=Unit&Category!=Steam"""
}

if ( Test-Path $Script:TestResultsDirectory ) {
	Remove-Item $Script:TestResultsDirectory -Recurse -Force
}

if ( Test-Path $Script:CoverageResultsDirectory ) {
	Remove-Item $Script:CoverageResultsDirectory -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $Script:TestResultsDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $Script:CoverageResultsDirectory | Out-Null

Invoke-NomadCommand $Script:DotNet @(
	"test",
	$Script:TestProjectPath,
	"--configuration",
	$Script:Configuration,
	"--no-build",
	"--filter",
	$filter,
	"--collect:XPlat Code Coverage",
	"--settings",
	$Script:CoverageSettingsPath,
	"--logger",
	"trx",
	"--results-directory",
	$Script:TestResultsDirectory
)

Write-NomadSection "Install ReportGenerator"

Invoke-NomadCommand $Script:DotNet @(
	"tool",
	"update",
	"--global",
	"dotnet-reportgenerator-globaltool"
)

$reportGenerator = "reportgenerator"

Write-NomadSection "Merge Coverage Reports"

Invoke-NomadCommand $reportGenerator @(
	"-reports:$Script:TestResultsDirectory/**/coverage.cobertura.xml",
	"-targetdir:$Script:CoverageResultsDirectory",
	"-reporttypes:Cobertura;HtmlSummary;MarkdownSummary"
)