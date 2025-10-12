# GitHub Actions Workflows

This repository uses two GitHub Actions workflows to automate building, testing, and exporting the Godot game.

> **Note:** Web deployment is not supported in Godot 4.x when using C#/.NET due to upstream limitations in .NET. Only Windows and Linux exports are available.

## Workflows Overview

### 1. CI - Build Validation (`ci.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests targeting `main` or `develop`

**Purpose:**
Validates that the project builds successfully on every code change.

**Steps:**
1. Checkout repository with Git LFS
2. Setup .NET 8 SDK
3. Setup Godot 4.3 .NET with export templates
4. Generate C# bindings
5. Restore .NET dependencies
6. Build project in Release configuration

**Duration:** ~5-10 minutes

**Status Badge:**
```markdown
![Build](https://github.com/mistalan/pixels-refactory/actions/workflows/ci.yml/badge.svg)
```

---

### 2. Export - Cross-Platform Builds (`export.yml`)

**Triggers:**
- Tags matching pattern `v*` (e.g., `v1.0.0`, `v2.1.3`)

**Purpose:**
Creates production builds for all supported platforms and attaches them to GitHub releases.

**Steps:**
1. Checkout repository with Git LFS
2. Setup .NET 8 SDK and Godot 4.3 .NET
3. Generate C# bindings and build project
4. Export builds for:
   - Windows Desktop (.exe)
   - Linux (.x86_64)
5. Package each build (ZIP for Windows, TAR.GZ for Linux)
6. Create GitHub release with all build artifacts

**Duration:** ~15-20 minutes

**Output Artifacts:**
- `PixelsRefactory-Windows.zip`
- `PixelsRefactory-Linux.tar.gz`

**Status Badge:**
```markdown
![Export](https://github.com/mistalan/pixels-refactory/actions/workflows/export.yml/badge.svg)
```

**Creating a Release:**
```bash
# Tag the current commit
git tag v1.0.0

# Push the tag to trigger the workflow
git push origin v1.0.0
```

---

## Setup Requirements

### Repository Secrets
No secrets are required. All workflows use `GITHUB_TOKEN` which is automatically provided.

---

## Troubleshooting

### Build Failures

**Issue:** C# bindings generation fails
```
Solution: Ensure Godot version matches exactly 4.3.0
```

**Issue:** .NET restore fails
```
Solution: Verify .NET 8 SDK is properly installed in the workflow
```

**Issue:** Export fails
```
Solution: Check that export_presets.cfg contains both presets:
- Windows Desktop
- Linux
```

---

## Workflow Dependencies

All workflows use these GitHub Actions:

- `actions/checkout@v4` - Repository checkout
- `actions/setup-dotnet@v4` - .NET SDK setup
- `chickensoft-games/setup-godot@v2` - Godot engine setup
- `ncipollo/release-action@v1` - Release creation (export.yml)

---

## Local Replication

To replicate CI builds locally:

```bash
# Generate C# bindings
godot --headless --build-solutions --quit

# Restore and build
dotnet restore
dotnet build --configuration Release
```

To replicate exports locally:

```bash
# Windows
godot --headless --export-release "Windows Desktop" ./export/windows/PixelsRefactory.exe

# Linux
godot --headless --export-release "Linux" ./export/linux/PixelsRefactory.x86_64
```

> **Note:** Web export is not available for C#/.NET projects in Godot 4.x

---

## Monitoring

View workflow status:
- [Actions Tab](https://github.com/mistalan/pixels-refactory/actions)
- Individual workflow runs show detailed logs
- Failed runs send notifications to repository watchers

---

**Note:** Export builds are stored in the `./export/` directory and are excluded from version control via `.gitignore`.
