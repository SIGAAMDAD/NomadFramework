Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$Script:ScriptsDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$Script:RepositoryRoot = Resolve-Path (Join-Path $Script:ScriptsDirectory "../..")

Set-Location $Script:RepositoryRoot

$Script:DotNet = if ( $env:DOTNET_EXE ) {
	$env:DOTNET_EXE
} else {
	"dotnet"
}

$Script:Configuration = if ( $env:NOMAD_CONFIGURATION ) {
	$env:NOMAD_CONFIGURATION
} else {
	"Release"
}

$Script:SourceDirectory = if ( $env:NOMAD_SOURCE_DIR ) {
	$env:NOMAD_SOURCE_DIR
} else {
	"Source"
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
	Assert-NomadPathExists -Path $Script:SourceDirectory -Description "Source directory"

	Get-ChildItem -Path $Script:SourceDirectory -Recurse -Filter "*.csproj" |
		Where-Object {
			$_.FullName -notmatch "[\\/](bin|obj)[\\/]"
		} |
		Sort-Object FullName
}

function Restore-NomadSourceProjects {
	Write-NomadSection "Restoring Source projects"

	$projects = @( Get-NomadSourceProjects )

	if ( $projects.Count -eq 0 ) {
		throw "No .csproj files were found under: $Script:SourceDirectory"
	}

	foreach ( $project in $projects ) {
		$relativePath = Resolve-Path -Relative $project.FullName

		Write-Host ""
		Write-Host "Restoring $relativePath"

		Invoke-NomadCommand `
			-File $Script:DotNet `
			-Arguments @(
				"restore",
				$project.FullName,
				"--nologo",
				"-p:Configuration=$Script:Configuration"
			)
	}
}

Write-NomadSection "NomadFramework Restore"
Write-Host "Repository Root: $Script:RepositoryRoot"
Write-Host "Source Directory: $Script:SourceDirectory"
Write-Host "Configuration: $Script:Configuration"
Write-Host "DotNet CLI: $Script:DotNet"

Restore-NomadSourceProjects