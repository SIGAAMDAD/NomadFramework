. "$PSScriptRoot/common.ps1"

Write-NomadSection "Clean"

$paths = @(
	"NuGet",
	"Build/Artifacts",
	"Tests/TestResults",
	"Tests/CoverageResults"
)

foreach ( $path in $paths ) {
	if ( Test-Path $path ) {
		Write-Host "Removing $path"
		Remove-Item $path -Recurse -Force
	}
}

$generatedDirectories = Get-ChildItem -Path "." -Recurse -Directory |
	Where-Object {
		$_.Name -eq "bin" -or
		$_.Name -eq "obj"
	}

foreach ( $directory in $generatedDirectories ) {
	Write-Host "Removing $($directory.FullName)"
	Remove-Item $directory.FullName -Recurse -Force
}