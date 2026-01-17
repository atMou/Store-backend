# CI/CD Workflows

This directory contains GitHub Actions workflows for continuous integration and continuous deployment.

## Workflows

### 1. CI/CD Pipeline (`ci.yml`)
**Triggers:** Push and Pull Requests to `main` and `develop` branches

**Jobs:**
- **Format Check**: Validates code formatting using `dotnet format`
- **Lint**: Runs code analysis and enforces code style rules
- **Build**: Compiles the entire solution
- **Test**: Runs all unit tests and collects code coverage
- **Code Coverage**: Generates coverage reports and adds comments to PRs

### 2. Auto Format Code (`format.yml`)
**Triggers:** Manual dispatch or scheduled (weekly on Mondays)

**Purpose:** Automatically formats code across the entire solution and commits changes

**Usage:**
```bash
# Trigger manually via GitHub Actions UI or:
gh workflow run format.yml
```

### 3. Pull Request Checks (`pr-validation.yml`)
**Triggers:** Pull request events (opened, synchronized, reopened)

**Features:**
- Comprehensive validation of formatting, build, and tests
- Automated PR comments with check results
- Detailed feedback on what needs to be fixed

### 4. Deploy to Production (`deploy.yml`)
**Triggers:** Push to `main` branch, version tags, or manual dispatch

**Purpose:** Build, test, and deploy the application to production

## Local Development

### Running Format Checks Locally

```bash
# Check formatting (dry run)
dotnet format "Store Backend.sln" --verify-no-changes

# Fix formatting issues
dotnet format "Store Backend.sln"
```

### Running Tests Locally

```bash
# Restore dependencies
dotnet restore "Store Backend.sln"

# Build solution
dotnet build "Store Backend.sln" --configuration Release

# Run tests with coverage
dotnet test "Store Backend.sln" --configuration Release --collect:"XPlat Code Coverage"
```

### Running Code Analysis Locally

```bash
# Build with code style enforcement
dotnet build "Store Backend.sln" --configuration Release /p:EnforceCodeStyleInBuild=true
```

## Configuration Files

### `.editorconfig`
Defines code style and formatting rules. The CI pipeline enforces these rules.

### `global.json`
Specifies the .NET SDK version to use (9.0.x).

## Code Coverage

Code coverage reports are generated for each test run and uploaded as artifacts. For pull requests, coverage summaries are automatically added as comments.

**Coverage Thresholds:**
- Warning: < 60%
- Good: 60-80%
- Excellent: > 80%

## Status Badges

Add these badges to your main README.md:

```markdown
![CI/CD Pipeline](https://github.com/YOUR_USERNAME/Store-Backend/actions/workflows/ci.yml/badge.svg)
![Code Format](https://github.com/YOUR_USERNAME/Store-Backend/actions/workflows/format.yml/badge.svg)
```

## Troubleshooting

### Formatting Failures
If the format check fails:
1. Run `dotnet format "Store Backend.sln"` locally
2. Commit and push the changes

### Build Failures
1. Check the build logs in GitHub Actions
2. Ensure all dependencies are properly restored
3. Verify the code compiles locally

### Test Failures
1. Run tests locally to reproduce the issue
2. Check test logs in the Actions tab
3. Ensure all required services (databases, etc.) are properly configured

## Best Practices

1. **Always run formatting before committing:**
   ```bash
   dotnet format "Store Backend.sln"
   ```

2. **Run tests locally before pushing:**
   ```bash
   dotnet test "Store Backend.sln"
   ```

3. **Keep the build passing:** Don't merge PRs with failing checks

4. **Monitor code coverage:** Aim to maintain or improve coverage with each PR

## Workflow Status

You can view the status of all workflows in the [Actions tab](../../actions) of the repository.
