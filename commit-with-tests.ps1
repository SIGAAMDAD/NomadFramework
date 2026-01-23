param(
    [string]$Message = "Auto-commit",
    [switch]$SkipTests = $false
)

# 1. Run tests locally first
if (-not $SkipTests) {
    Write-Host "Running tests locally..." -ForegroundColor Cyan
    
    # Option A: Using dotnet test
    $testResult = dotnet test --configuration Release --verbosity minimal
    
    # Option B: Using NUnit Console
    # $testResult = & "C:\Path\To\nunit3-console.exe" "YourTests.dll"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed! Aborting commit." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "All tests passed!" -ForegroundColor Green
}

# 2. Stage all changes
git add .

# 3. Commit
git commit -m $Message

# 4. Push to GitHub (triggers GitHub Actions)
git push origin HEAD

Write-Host "Commit pushed successfully!" -ForegroundColor Green