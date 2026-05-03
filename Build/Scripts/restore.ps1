Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$Script:ScriptsDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$Script:RepositoryRoot = Resolve-Path (Join-Path $Script:ScriptsDirectory "../..")

Set-Location $Script:RepositoryRoot

$Script:DotNet = if ( $env:DOTNET_EXE ) { $env:DOTNET_EXE } else { "dotnet" }

$Script:SolutionPath = if ( $env:NOMAD_SOLUTION ) {
	$env:NOMAD_SOLUTION
} else {
	"NomadFramework.slnx"
}

$Script:TestProjectPath = if ( $env:NOMAD_TEST_PROJECT ) {
	$env:NOMAD_TEST_PROJECT
} else {
	"Tests/Nomad.Tests.csproj"
}

$Script:Configuration = if ( $env:NOMAD_CONFIGURATION ) {
	$env:NOMAD_CONFIGURATION
} else {
	"Release"
}

$Script:CoverageSettingsPath = if ( $env:NOMAD_COVERAGE_SETTINGS ) {
	$env:NOMAD_COVERAGE_SETTINGS
} else {
	"coverlet.runsettings"
}

$Script:TestResultsDirectory = if ( $env:NOMAD_TEST_RESULTS_DIR ) {
	$env:NOMAD_TEST_RESULTS_DIR
} else {
	"Tests/TestResults"
}

$Script:CoverageResultsDirectory = if ( $env:NOMAD_COVERAGE_RESULTS_DIR ) {
	$env:NOMAD_COVERAGE_RESULTS_DIR
} else {
	"Tests/CoverageResults"
}

$Script:NuGetOutputDirectory = if ( $env:NOMAD_NUGET_OUTPUT_DIR ) {
	$env:NOMAD_NUGET_OUTPUT_DIR
} else {
	"NuGet"
}

function Write-NomadSection {
	param(
		[Parameter( Mandatory = $true )]
		[string] $Message
	)

	Write-Host ""
	Write-Host "========== $Message =========="
}

function Invoke-NomadCommand {
	param(
		[Parameter( Mandatory = $true )]
		[string] $File,

		[Parameter( Mandatory = $true )]
		[string[]] $Arguments
	)

	Write-Host ">> $File $($Arguments -join ' ')"

	& $File @Arguments

	if ( $LASTEXITCODE -ne 0 ) {
		throw "Command failed with exit code $LASTEXITCODE`: $File $($Arguments -join ' ')"
	}
}

function Assert-NomadPathExists {
	param(
		[Parameter( Mandatory = $true )]
		[string] $Path,

		[Parameter( Mandatory = $false )]
		[string] $Description = "Path"
	)

	if ( -not ( Test-Path $Path ) ) {
		throw "$Description does not exist: $Path"
	}
}

function Get-NomadSourceProjects {
	Get-ChildItem -Path "Source" -Recurse -Filter "*.csproj" |
		Where-Object { $_.FullName -notmatch "[\\/](bin|obj)[\\/]" } |
		Sort-Object FullName
}

function Get-NomadTestProjects {
	Get-ChildItem -Path "Tests" -Recurse -Filter "*.csproj" |
		Where-Object { $_.FullName -notmatch "[\\/](bin|obj)[\\/]" } |
		Sort-Object FullName
}