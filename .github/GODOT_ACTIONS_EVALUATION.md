# Evaluation of Godot GitHub Actions

## Overview

This document evaluates two popular Godot-related GitHub Actions available on the marketplace:
1. **godot-setup** by chickensoft-games
2. **godot-ci** by abarichello

## Actions Summary

### 1. godot-setup (chickensoft-games/setup-godot)

**Repository:** https://github.com/chickensoft-games/setup-godot  
**Marketplace:** https://github.com/marketplace/actions/setup-godot-action

#### What is it for?
- Sets up the Godot Engine directly on CI/CD runners (macOS, Windows, Linux)
- Designed for **headless use** in continuous integration environments
- Installs Godot as a native executable on the runner

#### Key Features:
- ✅ **C#/.NET Support**: Explicitly supports Godot .NET (Mono) builds
- ✅ **Export Templates**: Can install export templates via `include-templates: true`
- ✅ **Version Flexibility**: Supports specifying versions via `global.json` or directly in workflow
- ✅ **Cross-Platform**: Works on macOS, Windows, and Linux runners
- ✅ **Simple Setup**: Minimal configuration required
- ✅ **Active Maintenance**: Well-maintained by chickensoft-games

#### Example Usage:
```yaml
- uses: chickensoft-games/setup-godot@v2
  with:
    version: '4.5.0'
    use-dotnet: true
    include-templates: true
```

---

### 2. godot-ci (abarichello/godot-ci)

**Repository:** https://github.com/abarichello/godot-ci  
**Marketplace:** https://github.com/marketplace/actions/godot-ci

#### What is it for?
- Provides **Docker images** for exporting Godot Engine games
- Offers pre-configured templates for various deployment targets
- Designed for GDScript projects primarily

#### Key Features:
- ✅ **Docker-Based**: Uses containerized builds
- ✅ **Deployment Templates**: Includes ready-to-use templates for:
  - GitLab Pages
  - GitHub Pages
  - Itch.io
- ✅ **Multiple Platforms**: Supports HTML5, Windows, Linux, macOS, Android, iOS exports
- ⚠️ **No C#/.NET Support**: Does not support Godot .NET (Mono) builds
- ⚠️ **Complex Setup**: Requires Docker knowledge and more configuration
- ⚠️ **Compilation Required**: May need to compile Godot with specific modules

#### Example Usage:
```yaml
- uses: abarichello/godot-ci@v3
  with:
    godot_version: 4.2.0
```

---

## Comparison Matrix

| Feature | godot-setup (chickensoft) | godot-ci (abarichello) |
|---------|---------------------------|------------------------|
| **C#/.NET Support** | ✅ Yes | ❌ No |
| **Export Templates** | ✅ Yes | ✅ Yes (via Docker) |
| **Deployment Templates** | ❌ No | ✅ Yes (Pages, Itch.io) |
| **Docker-Based** | ❌ No (native) | ✅ Yes |
| **Setup Complexity** | Low | Medium-High |
| **Godot 4.x Support** | ✅ Yes | ✅ Yes |
| **Platform Support** | macOS, Windows, Linux | Linux (via Docker) |
| **Active Maintenance** | ✅ Yes | ✅ Yes |
| **Best For** | C#/.NET projects | GDScript projects |

---

## Current Workflow Analysis

### Our Current Setup

**Workflows:**
1. `ci.yml` - Build validation (push/PR)
2. `export.yml` - Cross-platform exports (tags)

**Current Action Usage:**
- ✅ Already using `chickensoft-games/setup-godot@v2`
- ✅ Already using `actions/setup-dotnet@v5`
- ✅ Already using `actions/checkout@v5`
- ✅ Already using `ncipollo/release-action@v1`

**Configuration:**
```yaml
- uses: chickensoft-games/setup-godot@v2
  with:
    version: '4.5.0'
    use-dotnet: true
    include-templates: true
```

### What We're Doing Right

1. **Correct Action Choice**: We're using `godot-setup` which is the **right choice** for C#/.NET projects
2. **Proper Configuration**: 
   - `use-dotnet: true` enables C# support
   - `include-templates: true` allows exports
   - Explicit version pinning (`4.5.0`)
3. **Clean Setup**: Direct installation on runner (no Docker complexity)
4. **Complete CI/CD**: Both build validation and release exports are covered

---

## Evaluation Results

### Should We Use godot-ci (abarichello)?

**❌ NO** - Not recommended for this project

**Reasons:**
1. **No C#/.NET Support**: This is a dealbreaker since our project uses Godot 4.5 .NET
2. **Docker Overhead**: Adds unnecessary complexity when native setup works fine
3. **Redundant**: We already have working export functionality with `godot-setup`
4. **Deployment Templates Not Needed**: 
   - We don't use GitLab Pages
   - We don't use GitHub Pages for game hosting
   - We don't publish to Itch.io (currently)
   - We use GitHub Releases instead

### Should We Continue Using godot-setup (chickensoft)?

**✅ YES** - Already optimal for our needs

**Reasons:**
1. **Perfect Fit**: Designed specifically for C#/.NET Godot projects
2. **Working Solution**: Currently functioning without issues
3. **Simple**: No Docker complexity
4. **Well-Maintained**: Active development and support
5. **Community Trust**: Widely used in Godot .NET community

---

## Recommendations

### Current State: ✅ Optimal

Our current workflow setup is **already optimal** for this project. No changes needed.

### Why Our Setup Is Good:

1. **Right Tool for the Job**: `godot-setup` is purpose-built for C#/.NET projects
2. **Complete Automation**:
   - CI validates every push/PR
   - Tags trigger automatic exports
   - Releases are created with artifacts
3. **Performance**: Native installation is faster than Docker
4. **Maintainability**: Simple configuration, easy to understand
5. **Flexibility**: Easy to update Godot versions

### Potential Future Considerations:

If project requirements change, consider `godot-ci` only if:
- ❌ Switching from C#/.NET to GDScript (not planned)
- ❌ Need HTML5/Web export (not supported in Godot 4.x with C# anyway)
- ❌ Want to publish to Itch.io (could be useful for distribution)
- ❌ Need very specific deployment automation templates

### Optional Enhancements (Not Required):

While our current setup is optimal, these minor improvements could be considered:

1. **Cache .NET packages** to speed up builds:
   ```yaml
   - uses: actions/cache@v3
     with:
       path: ~/.nuget/packages
       key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
   ```

2. **Add artifact retention** for CI builds (debugging):
   ```yaml
   - uses: actions/upload-artifact@v3
     if: failure()
     with:
       name: build-logs
       path: .godot/mono/build_logs/
   ```

3. **Matrix builds** for multiple Godot versions (if testing compatibility):
   ```yaml
   strategy:
     matrix:
       godot: ['4.5.0', '4.5.1']
   ```

However, these are **not necessary** at this time given the project's current size and scope.

---

## Conclusion

### Final Answer

**Q: Should we integrate godot-ci or make any changes?**  
**A: No.** Our current setup using `godot-setup` is already optimal for this C#/.NET Godot project.

### Summary

| Action | Status | Reason |
|--------|--------|--------|
| `godot-setup` (chickensoft) | ✅ **Keep using** | Perfect for C#/.NET, already integrated |
| `godot-ci` (abarichello) | ❌ **Do not use** | Incompatible with C#/.NET projects |

### Key Takeaways

1. We're already using the **best action** for our needs (`godot-setup`)
2. `godot-ci` is **not suitable** for C#/.NET projects
3. Our current workflows are **well-configured** and require no changes
4. The combination of `godot-setup` + `setup-dotnet` is the **standard approach** for Godot .NET projects

---

**Evaluation Date:** 2025-10-12  
**Project:** Pixels Refactory  
**Godot Version:** 4.5.0 .NET  
**Evaluator:** GitHub Copilot
