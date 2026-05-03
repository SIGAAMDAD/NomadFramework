Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$Script:ScriptsDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$Script:RepositoryRoot = Resolve-Path ( Join-Path $Script:ScriptsDirectory "../.." )

Set-Location $Script:RepositoryRoot

$Script:DotNet = if ( $env:DOTNET_EXE ) {
	$env:DOTNET_EXE
} else {
	"dotnet"
}

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
	"Debug"
}

$Script:RestoreLockedMode = if ( $env:NOMAD_RESTORE_LOCKED_MODE ) {
	$env:NOMAD_RESTORE_LOCKED_MODE
} else {
	"false"
}

$Script:RestoreTools = if ( $env:NOMAD_RESTORE_TOOLS ) {
	$env:NOMAD_RESTORE_TOOLS
} else {
	"true"
}

$Script:RestoreTests = if ( $env:NOMAD_RESTORE_TESTS ) {
	$env:NOMAD_RESTORE_TESTS
} else {
	"true"
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

function Get-NomadToolProjects {
	if ( -not ( Test-Path "Tools" ) ) {
		return @()
	}

	return Get-ChildItem -Path "Tools" -Recurse -Filter "*.csproj" |
		Where-Object { $_.FullName -notmatch "[\\/](bin|obj)[\\/]" } |
		Sort-Object FullName
}

function Get-NomadRestoreArguments {
	param(
		[Parameter( Mandatory = $true )]
		[string] $ProjectOrSolution
	)

	$arguments = @(
		"restore",
		$ProjectOrSolution,
		"-p:Configuration=$Script:Configuration"
	)

	if ( $Script:RestoreLockedMode -eq "true" ) {
		$arguments += "--locked-mode"
	}

	return $arguments
}

Write-NomadSection "Restore configuration"

Write-Host "RepositoryRoot:     $Script:RepositoryRoot"
Write-Host "SolutionPath:       $Script:SolutionPath"
Write-Host "TestProjectPath:    $Script:TestProjectPath"
Write-Host "Configuration:      $Script:Configuration"
Write-Host "RestoreTests:       $Script:RestoreTests"
Write-Host "RestoreTools:       $Script:RestoreTools"
Write-Host "RestoreLockedMode:  $Script:RestoreLockedMode"

Write-NomadSection "Validate restore inputs"

Assert-NomadPathExists -Path $Script:SolutionPath -Description "Solution"

if ( $Script:RestoreTests -eq "true" ) {
	Assert-NomadPathExists -Path $Script:TestProjectPath -Description "Test project"
}

Write-NomadSection "Restore solution"

Invoke-NomadCommand `
	-File $Script:DotNet `
	-Arguments ( Get-NomadRestoreArguments -ProjectOrSolution $Script:SolutionPath )

if ( $Script:RestoreTests -eq "true" ) {
	Write-NomadSection "Restore test project"

	Invoke-NomadCommand `
		-File $Script:DotNet `
		-Arguments ( Get-NomadRestoreArguments -ProjectOrSolution $Script:TestProjectPath )
}

if ( $Script:RestoreTools -eq "true" ) {
	$toolProjects = @( Get-NomadToolProjects )

	if ( $toolProjects.Count -gt 0 ) {
		Write-NomadSection "Restore tools"

		foreach ( $toolProject in $toolProjects ) {
			$relativePath = Resolve-Path -Path $toolProject.FullName -Relative

			Invoke-NomadCommand `
				-File $Script:DotNet `
				-Arguments ( Get-NomadRestoreArguments -ProjectOrSolution $relativePath )
		}
	}
}

Write-NomadSection "Restore complete"