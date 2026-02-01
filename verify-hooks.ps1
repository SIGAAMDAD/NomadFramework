# Check if hooks are configured
$hookPath = git config --get core.hooksPath
if (-not $hookPath) {
    Write-Host "❌ Git hooks not configured!" -ForegroundColor Red
    Write-Host "Run: git config core.hooksPath .githooks" -ForegroundColor Yellow
    exit 1
}

# Test hook manually
Write-Host "Testing pre-commit hook..." -ForegroundColor Cyan
& ".githooks/pre-commit"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Hooks are working correctly!" -ForegroundColor Green
} else {
    Write-Host "❌ Hook execution failed" -ForegroundColor Red
}