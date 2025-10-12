# GitHub Actions Workflows

This repository uses three GitHub Actions workflows to automate building, testing, and deploying the Godot game.

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
   - Web (HTML5/WASM)
5. Package each build (ZIP for Windows/Web, TAR.GZ for Linux)
6. Create GitHub release with all build artifacts

**Duration:** ~15-25 minutes

**Output Artifacts:**
- `PixelsRefactory-Windows.zip`
- `PixelsRefactory-Linux.tar.gz`
- `PixelsRefactory-Web.zip`

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

### 3. Web Deploy - GitHub Pages (`web-deploy.yml`)

**Triggers:**
- Push to `main` branch
- Manual workflow dispatch

**Purpose:**
Exports the Web build and deploys it to GitHub Pages for live demos.

**Steps:**
1. Export Web build to `./export/web`
2. Add `.nojekyll` file for GitHub Pages compatibility
3. Upload build artifact
4. Deploy to GitHub Pages environment

**Duration:** ~10-15 minutes

**Live URL:**
Once deployed, the game will be available at:
```
https://mistalan.github.io/pixels-refactory/
```

**Manual Trigger:**
Go to Actions → Web Deploy → Run workflow

---

## Setup Requirements

### Repository Secrets
No secrets are required. All workflows use `GITHUB_TOKEN` which is automatically provided.

### GitHub Pages Setup
To enable the Web Deploy workflow:

1. Go to repository Settings → Pages
2. Under "Source", select "GitHub Actions"
3. Save the configuration

The next push to `main` will automatically deploy to Pages.

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
Solution: Check that export_presets.cfg contains all three presets:
- Windows Desktop
- Linux
- Web
```

### GitHub Pages Not Working

**Issue:** 404 error after deployment
```
Solution: 
1. Check Pages settings (Settings → Pages → Source: GitHub Actions)
2. Verify workflow completed successfully
3. Wait 1-2 minutes for DNS propagation
```

---

## Workflow Dependencies

All workflows use these GitHub Actions:

- `actions/checkout@v4` - Repository checkout
- `actions/setup-dotnet@v4` - .NET SDK setup
- `chickensoft-games/setup-godot@v2` - Godot engine setup
- `ncipollo/release-action@v1` - Release creation (export.yml)
- `actions/upload-pages-artifact@v3` - Pages upload (web-deploy.yml)
- `actions/deploy-pages@v4` - Pages deployment (web-deploy.yml)

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

# Web
godot --headless --export-release "Web" ./export/web/index.html
```

---

## Monitoring

View workflow status:
- [Actions Tab](https://github.com/mistalan/pixels-refactory/actions)
- Individual workflow runs show detailed logs
- Failed runs send notifications to repository watchers

---

**Note:** Export builds are stored in the `./export/` directory and are excluded from version control via `.gitignore`.
