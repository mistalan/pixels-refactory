# Copilot Instructions

> **Read me first (Agent):** Trust these instructions. Only search the repo if something here is missing or demonstrably wrong.

## Summary

This repository contains a **2D, node-graph factory simulation** (EPK/BPMN-lite) built with **Godot 4.3 (.NET/C#)**.  
Players assemble a production pipeline for “software that powers the music industry” (e.g., IDE → Tests → CI → Deploy → Music Ops).  
Core gameplay is a **tick-based simulation** flowing items over graph edges between typed nodes (Event, Function, Gateway, Buffer, Sink).

## High-Level Details

- **Project type:** Godot 4.3 **.NET** (C# 11 / .NET 8) 2D game with a custom graph editor (Godot `GraphEdit`/`GraphNode`).
- **Languages:** C# (gameplay & tools), JSON (data/content), GDScript optional for editor glue (minimized).
- **Target runtimes:** Windows, Linux, Web (HTML5/WASM). Native exports are primary; Web is optional.
- **Repo size expectation:** Small (few hundred files); one Godot project with lightweight assets.
- **Design tenets:**  
  - Deterministic, **discrete ticks** (e.g., 10/s).  
  - Acyclic graphs (editor validation; topological order per tick).  
  - Data-driven content (`/Game/Data/*.json`).

---

## Build & Validate (do this, in order)

> **Always** use the **.NET build** of Godot 4.3 and **.NET 8** SDK.

### 0) Bootstrap (local)
1. Install **.NET 8 SDK**.  
2. Install **Godot 4.3 .NET** + **Export Templates** (matching version).  
3. Open the project in Godot; on first C# load, let it generate MSBuild files.

### 1) Clean build (local CLI)
```bash
# Generate/refresh C# bindings (safe to run anytime)
godot --headless --build-solutions --quit

# Restore and build C# (Release)
dotnet restore
dotnet build --configuration Release
```
**Preconditions:** run from repo root where `project.godot` lives.  
**Postconditions:** Build succeeds, DLLs output to Godot’s `.mono` artifacts.  
**Gotchas:** If bindings are stale, the first `dotnet build` may fail → run `godot --headless --build-solutions` again.

### 2) Run (local)
- **Editor play:** Open in Godot → press ▶.  
- **Headless sim smoke test (optional):**
```bash
godot --headless --quit # Should exit 0 if project loads
```

### 3) Test (currently minimal/manual)
- No formal unit tests yet; validation is via **simulation smoke**: load sample graph JSON and run 10–30 seconds without error.  
- When tests exist, they will be under `/Game/Tests` (C# xUnit or Godot C# test scene). **Always run tests before PRs.**

### 4) Lint/format
- Use IDE’s C# formatter (default conventions). If `EditorConfig` exists at repo root, **always** respect it.  
- Godot scene diffs: **avoid** noisy changes; don’t re-save unrelated scenes.

### 5) Export / CI validation
- Exports are driven by **Godot Export Presets** (`export_presets.cfg`).  
- CI uses GitHub Actions to validate that presets can export (Windows/Linux/Web) and attach artifacts on tags.

**Timing notes:**  
- `godot --headless --build-solutions` typically < 30s on CI; `dotnet build` < 60s for small changes; full export per target 1–3 min.

---

## Project Layout & Architecture

```
/Game
  /Scenes
    GraphScene.tscn          # Main scene hosting GraphEdit + HUD
    Nodes/                   # Godot scene prefabs for Event/Function/Sink/Gateway/Buffer
  /Scripts
    /Simulation
      Core/                  # SimContext, TickLoop, Edge/Queue models, Item types
      Nodes/                 # EventNode.cs, FunctionNode.cs, GatewayNode.cs, BufferNode.cs, SinkNode.cs
      Systems/               # TopologicalSort, Validation (acyclic), Scheduler
    /GraphEditor             # GraphEdit wiring, node creation, connect/disconnect handlers
    /UI                      # HUD, KPI panels, play/pause, speed
  /Data
    sample_graph.json        # Minimal playable graph (Tickets -> Code -> Test -> Sink)
project.godot
export_presets.cfg
README.md
```

### Major architectural elements
- **SimContext**: in-memory world state; owns node registry, edge queues, tick rate.  
- **Node logic interface**: each node implements `Tick(SimContext)`; pulls from input queues, pushes to outputs.  
- **Deterministic tick**: process nodes in **topological order**; cycles are rejected by editor validation.  
- **Items**: simple record with `Type`, `Quality`, optional `Size`.  
- **Config/data**: JSON defines nodes/edges & per-node config (throughput, latency, transforms).

### Key configs
- `project.godot`: Godot project configuration.  
- `export_presets.cfg`: export targets (Windows/Linux/Web). **Required for CI export.**

---

## Checks before check-in (CI/CD)

GitHub Actions (expected under `.github/workflows/`):

1) **`ci.yml`** — Build validation on `push`/`PR`
- Steps:
  - `actions/setup-dotnet@v4` (8.0.x)
  - `chickensoft-games/setup-godot@v2` with `version: 4.3`, `use-dotnet: true`, `include-templates: true`
  - `godot --headless --build-solutions --quit`
  - `dotnet build -c Release`
- **Replicate locally**: run the same 3 commands in the same order.

2) **`export.yml`** — Tagged release exports
- Trigger: tags `v*`
- Steps:
  - Checkout
  - `firebelley/godot-export@v7` with **mono** Godot + templates URLs for 4.3
  - Archive exports; attach via `ncipollo/release-action@v1`

3) **`web-deploy.yml`** (optional) — Export preset `Web` → GitHub Pages
- On `main` push, export preset `Web`, then `actions/deploy-pages@v4`.

> **Agent:** When adding a new workflow, mirror these patterns. Always ensure export presets exist before attempting export.

---

## Conventions & Dependencies

- **Godot Version lock:** 4.3 (Mono). Upgrading Godot requires aligning **both** editor and export templates; update CI URLs accordingly.  
- **.NET:** 8.0.x. Don’t switch language level without updating `global.json` / project files.  
- **Graph invariants:** Graph must be acyclic; editor must block cycles.  
- **Item flow:** FIFO per edge queue; throughput/latency are per-node config.  
- **Data files:** JSON in `/Game/Data`. Keep them small; version with the code.  
- **Assets:** Keep 2D textures minimal; no heavy third-party packages by default.

---

## Root Contents (expected)

- `project.godot` — Godot project file.  
- `export_presets.cfg` — Export targets.  
- `/Game/**` — All scenes, scripts, data.  
- `README.md` — Quick start + screenshots/gifs (keep updated).  
- `.editorconfig` (optional) — C# formatting rules.  
- `.github/workflows/*.yml` — CI, Export, Web deploy.  
- `.github/copilot-instructions.md` — this file.

**README.md (expected content):**  
- What the game is (1–2 paragraphs).  
- How to run locally (Godot version, commands).  
- How to build/export.  
- Controls: play/pause, speed controls.  
- Contributing: branches, PR checks (CI must pass).

---

## Snippets from key source files (indicative)

```csharp
// /Game/Scripts/Simulation/Core/Item.cs
public enum ItemType { Ticket, Spec, Code, Test, Build, Doc }
public sealed class Item { public ItemType Type; public float Quality = 1f; }
```

```csharp
// /Game/Scripts/Simulation/Core/INodeLogic.cs
public interface INodeLogic {
  string Id { get; }
  void Tick(SimContext ctx);
}
```

```csharp
// /Game/Scripts/Simulation/Nodes/FunctionNode.cs
public sealed class FunctionNode : INodeLogic {
  public string Id { get; init; }
  public int Throughput { get; init; } = 1;
  public int Latency { get; init; } = 0;
  public void Tick(SimContext ctx) {
    // Pull up to Throughput from inputs, process, then enqueue outputs.
  }
}
```

```csharp
// /Game/Scripts/Simulation/Systems/TopologicalSort.cs
// Ensures deterministic node execution order; editor blocks cycles.
```

---

## Known pitfalls & workarounds

- **Build fails with missing Godot assemblies:** Run `godot --headless --build-solutions` before `dotnet build`.  
- **Export fails on CI:** Ensure `export_presets.cfg` exists and matches Godot version; verify template URLs.  
- **Noisy scene diffs:** Avoid opening/saving unrelated scenes; keep Godot version consistent across contributors.

---

## Final instructions to the Agent

- **Always** follow the **Build & Validate** order above.  
- Prefer **C#** for gameplay logic; keep GDScript minimal.  
- When adding features, place code in the indicated folders and keep the tick loop deterministic.  
- Only perform repo-wide searches if these instructions are incomplete or produce errors.
