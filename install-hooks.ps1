# Create .githooks directory
New-Item -ItemType Directory -Force -Path .githooks

# Copy hooks
$hooks = @('pre-commit', 'pre-push', 'commit-msg')
foreach ($hook in $hooks) {
    $source = ".\scripts\hooks\$hook"
    $dest = ".\githooks\$hook"
    
    if (Test-Path $source) {
        Copy-Item -Path $source -Destination $dest -Force
        Write-Host "Installed $hook hook" -ForegroundColor Green
    }
}

# Make executable (Unix)
if (Get-Command chmod -ErrorAction SilentlyContinue) {
    chmod +x .githooks/*
}

# Configure git
git config core.hooksPath .githooks

Write-Host "✅ Hooks installed successfully!" -ForegroundColor Green
Write-Host "Run: git commit -m 'test' to trigger" -ForegroundColor Cyan