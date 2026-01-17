# Developer Environment Setup Script
# This script sets up pre-commit hooks and other development tools

Write-Host "?? Setting up development environment..." -ForegroundColor Cyan

$RepoRoot = git rev-parse --show-toplevel
if (-not $RepoRoot) {
    Write-Host "? Not in a git repository" -ForegroundColor Red
    exit 1
}

# Check prerequisites
Write-Host "`n?? Checking prerequisites..." -ForegroundColor Yellow

# Check .NET SDK
if (Get-Command dotnet -ErrorAction SilentlyContinue) {
    $dotnetVersion = dotnet --version
    Write-Host "? .NET SDK installed: $dotnetVersion" -ForegroundColor Green
} else {
    Write-Host "? .NET SDK not found. Please install .NET 9.0 SDK" -ForegroundColor Red
    exit 1
}

# Check Git
if (Get-Command git -ErrorAction SilentlyContinue) {
    $gitVersion = git --version
    Write-Host "? Git installed: $gitVersion" -ForegroundColor Green
} else {
    Write-Host "? Git not found" -ForegroundColor Red
    exit 1
}

# Install pre-commit hooks
Write-Host "`n?? Setting up Git hooks..." -ForegroundColor Yellow

$hooksPath = Join-Path $RepoRoot ".github\scripts"
git config core.hooksPath $hooksPath

Write-Host "? Git hooks path configured: $hooksPath" -ForegroundColor Green

# Make scripts executable (if on Unix-like system)
if ($IsLinux -or $IsMacOS) {
    chmod +x (Join-Path $hooksPath "pre-commit")
    Write-Host "? Made pre-commit script executable" -ForegroundColor Green
}

# Restore NuGet packages
Write-Host "`n?? Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore "Store Backend.sln"

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Packages restored successfully" -ForegroundColor Green
} else {
    Write-Host "?? Package restore had warnings" -ForegroundColor Yellow
}

# Install dotnet tools
Write-Host "`n?? Installing dotnet tools..." -ForegroundColor Yellow

# Check if dotnet format is available
$formatVersion = dotnet format --version 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "? dotnet format is available: $formatVersion" -ForegroundColor Green
} else {
    Write-Host "?? dotnet format not found (it's included in .NET SDK 6.0+)" -ForegroundColor Yellow
}

# Run initial format check
Write-Host "`n?? Running initial format check..." -ForegroundColor Yellow
dotnet format "Store Backend.sln" --verify-no-changes --verbosity quiet

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Code is properly formatted" -ForegroundColor Green
} else {
    Write-Host "?? Code formatting issues detected" -ForegroundColor Yellow
    $response = Read-Host "Do you want to auto-format now? (y/n)"
    if ($response -eq 'y' -or $response -eq 'Y') {
        dotnet format "Store Backend.sln"
        Write-Host "? Code formatted successfully" -ForegroundColor Green
    }
}

# Summary
Write-Host "`n? Setup complete!" -ForegroundColor Cyan
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  1. Pre-commit hooks are now active"
Write-Host "  2. Run 'dotnet format `"Store Backend.sln`"' to format code"
Write-Host "  3. Run 'dotnet test `"Store Backend.sln`"' to run tests"
Write-Host "  4. Read .github/workflows/README.md for CI/CD documentation"
Write-Host "`nHappy coding! ??" -ForegroundColor Cyan
