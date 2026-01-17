# Pre-commit hook to run formatting and tests
# Install: Copy this file to .git/hooks/pre-commit.ps1
# Configure git: git config core.hooksPath .github/scripts

Write-Host "?? Running pre-commit checks..." -ForegroundColor Cyan

$SolutionFile = "Store Backend.sln"

# Check if dotnet is installed
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "? dotnet CLI is not installed" -ForegroundColor Red
    exit 1
}

# Run format check
Write-Host "?? Checking code formatting..." -ForegroundColor Yellow
dotnet format $SolutionFile --verify-no-changes --verbosity quiet
$FormatExitCode = $LASTEXITCODE

if ($FormatExitCode -ne 0) {
    Write-Host "? Code formatting issues detected!" -ForegroundColor Red
    Write-Host "?? Run 'dotnet format `"$SolutionFile`"' to fix formatting" -ForegroundColor Yellow
    
    $response = Read-Host "Do you want to auto-format now? (y/n)"
    if ($response -eq 'y' -or $response -eq 'Y') {
        dotnet format $SolutionFile
        Write-Host "? Code formatted. Please review changes and commit again." -ForegroundColor Green
    }
    exit 1
}

# Build the solution
Write-Host "?? Building solution..." -ForegroundColor Yellow
dotnet build $SolutionFile --configuration Release --verbosity quiet --nologo
$BuildExitCode = $LASTEXITCODE

if ($BuildExitCode -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}

# Run tests
Write-Host "?? Running tests..." -ForegroundColor Yellow
dotnet test $SolutionFile --configuration Release --no-build --verbosity quiet --nologo
$TestExitCode = $LASTEXITCODE

if ($TestExitCode -ne 0) {
    Write-Host "? Tests failed!" -ForegroundColor Red
    exit 1
}

Write-Host "? All pre-commit checks passed!" -ForegroundColor Green
exit 0
