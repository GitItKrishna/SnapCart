# Code Quality Integration Setup Guide

This repository is configured with comprehensive code quality checks, security scanning, and automated dependency management through GitHub Actions.

## Features Enabled

### 1. **Continuous Integration (CI Pipeline)**
- **Automated Builds**: Every push and PR triggers a build
- **Unit Tests**: Tests run automatically with coverage reporting
- **Code Coverage**: Coverage reports uploaded to Codecov
- **Multi-stage pipeline**: Build, test, and quality checks

**Workflow**: `.github/workflows/ci.yml`

### 2. **Code Quality Analysis**
- **SonarQube Integration**: Advanced code quality metrics
- **Complexity Analysis**: Identify complex code sections
- **Code Smells Detection**: Best practice violations
- **Bug Detection**: Potential runtime issues

**Workflow**: `.github/workflows/ci.yml` (code-quality job)

### 3. **Security Scanning**
- **CodeQL**: GitHub's native code security analysis
- **Trivy**: Container and filesystem vulnerability scanning
- **OWASP Dependency-Check**: Identifies known CVEs in dependencies
- **Dependabot**: Automated security updates for packages

**Workflows**: 
- `.github/workflows/codeql.yml`
- `.github/workflows/ci.yml` (security-scan job)

### 4. **Dependency Management**
- **Dependabot**: Automatic dependency updates
- **Outdated Package Detection**: Identifies outdated NuGet packages
- **Security Patches**: Automatic PRs for security updates

**Configuration**: `.github/dependabot.yml`

## Setup Instructions

### Step 1: Enable GitHub Security Features

1. Go to your repository on GitHub
2. Navigate to **Settings → Security and analysis**
3. Enable the following:
   - ✅ **Dependabot alerts**
   - ✅ **Dependabot security updates**
   - ✅ **Code scanning** (already enabled via CodeQL workflow)
   - ✅ **Secret scanning**

### Step 2: Configure SonarQube (Optional but Recommended)

1. **Create a SonarQube account** (free tier available at [SonarCloud](https://sonarcloud.io))
2. **Add organization and project** in SonarCloud
3. **Generate token**: SonarCloud → Account → Security → Generate token
4. **Add GitHub Secrets**:
   - Go to your repo → **Settings → Secrets and variables → Actions**
   - Add new secrets:
     - `SONAR_HOST_URL`: `https://sonarcloud.io`
     - `SONAR_TOKEN`: (paste your SonarCloud token)

### Step 3: Configure Codecov (Optional)

1. Visit [codecov.io](https://codecov.io)
2. Connect your GitHub account
3. Select your repository to enable coverage tracking
4. No secrets needed - Codecov auto-detects pushes from GitHub

### Step 4: Add Code Coverage to Your Projects (Required for Tests)

For unit test coverage to work, ensure your test projects have Coverlet NuGet package:

```bash
dotnet add YourTests.csproj package coverlet.collector
```

Or add to your test `.csproj`:
```xml
<ItemGroup>
  <PackageReference Include="coverlet.collector" Version="*" />
</ItemGroup>
```

### Step 5: Create Unit Tests (Recommended)

Create test projects following this pattern:
```
Snapcart-Distributed/
├── Catalog/
├── Catalog.Tests/        ← New test project
│   └── Catalog.Tests.csproj
└── ...
```

Example test project (.csproj):
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="*" />
    <PackageReference Include="coverlet.collector" Version="*" />
    <PackageReference Include="Moq" Version="*" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Catalog\Catalog.csproj" />
  </ItemGroup>
</Project>
```

## What Happens on Push/PR

1. **GitHub Actions triggers** → Builds and tests your code
2. **CodeQL scans** → Analyzes code for security issues
3. **Trivy scans** → Checks for vulnerabilities
4. **Coverage reports** → Uploaded to Codecov
5. **Results appear** in PR checks before merge

## Viewing Results

### GitHub UI
- **Settings → Security and analysis**: See all security features
- **Pull Requests**: View checks and status before merge
- **Issues**: Security vulnerabilities appear here

### External Dashboards
- **Codecov**: [codecov.io](https://codecov.io) - View coverage trends
- **SonarCloud**: [sonarcloud.io](https://sonarcloud.io) - View code quality metrics

## Status Badges (Optional)

Add these to your `README.md`:

```markdown
[![CI Pipeline](https://github.com/GitItKrishna/SnapCart/actions/workflows/ci.yml/badge.svg)](https://github.com/GitItKrishna/SnapCart/actions/workflows/ci.yml)
[![CodeQL](https://github.com/GitItKrishna/SnapCart/actions/workflows/codeql.yml/badge.svg)](https://github.com/GitItKrishna/SnapCart/actions/workflows/codeql.yml)
[![codecov](https://codecov.io/gh/GitItKrishna/SnapCart/branch/master/graph/badge.svg)](https://codecov.io/gh/GitItKrishna/SnapCart)
```

## Troubleshooting

### Tests not running?
- Ensure test projects have `.Tests.csproj` naming or `<IsTestProject>true</IsTestProject>` in .csproj
- Check that tests are executable: `dotnet test`

### SonarQube not analyzing?
- Verify `SONAR_TOKEN` is correctly set in GitHub Secrets
- Check SonarCloud for any errors in recent analyses

### Coverage not reported?
- Ensure Coverlet is installed: `dotnet list package --outdated`
- Run tests locally: `dotnet test /p:CollectCoverage=true`

## Next Steps

1. ✅ Create unit tests for critical paths
2. ✅ Push changes and monitor GitHub Actions
3. ✅ Review CodeQL findings in Security tab
4. ✅ Configure branch protection rules requiring quality checks
5. ✅ Set up SonarQube for detailed code quality metrics

---

For more info, see:
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [CodeQL Documentation](https://codeql.github.com/docs/)
- [Dependabot Documentation](https://docs.github.com/en/code-security/dependabot)

