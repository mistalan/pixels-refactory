# 🏭 Pixel's Refactory
### _Automate Creativity._

[![Godot](https://img.shields.io/badge/Godot-4.5%20.NET-blue?logo=godot-engine&logoColor=white)](https://godotengine.org)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Build](https://github.com/mistalan/pixels-refactory/actions/workflows/ci.yml/badge.svg)](https://github.com/mistalan/pixels-refactory/actions/workflows/ci.yml)
[![Export](https://github.com/mistalan/pixels-refactory/actions/workflows/export.yml/badge.svg)](https://github.com/mistalan/pixels-refactory/actions/workflows/export.yml)

---

## 📚 Table of Contents
- [ℹ️ About the Game](#ℹ️-about-the-game)
- [✨ Features](#✨-features)
- [🛠️ Tech Stack](#️-tech-stack)
- [📁 Project Structure](#-project-structure)
- [🚀 Build & Run](#-build--run)
- [⚙️ Continuous Integration](#️-continuous-integration)
- [📦 Creating a Release](#-creating-a-release)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [🗺️ Roadmap](#️-roadmap)

---

## ℹ️ About the Game

> Build, connect, and optimize the world's first creative factory.  
> Automate coding, testing and music production — but be careful:  
> every refactor changes the rhythm of the world.

**Pixel's Refactory** is a 2D factory-simulation game built with **Godot 4.5 (.NET)**.  
You design pipelines of **software processes** — Events, Functions, Tests, and Deployments —  
and feed them into a living world where code and creativity intertwine.

Your tiny companion **Pixel**, a curious digital spider, rewards precision and focus  
through playful mini-games and insights into the art of automation.  
Research new technologies like **Kusintel**, an AI-assistant that helps you  
refactor both code and reality.

---

## ✨ Features
- 🎨 **Node-Graph Factory Editor** — design event-driven production lines in an EPK-style flowchart  
- ⏱️ **Tick-Based Simulation** — deterministic throughput, latency, and quality metrics  
- 🎵 **Music Integration** — code meets rhythm; automation becomes art  
- 🤖 **AI Research Tree** — unlock Kusintel for smarter automation  
- 🎮 **Pixel Mini-Games** — gain focus buffs and productivity boosts  
- 📊 **Data-Driven JSON System** — create your own nodes, edges and item types

---

## 🛠️ Tech Stack
| Area | Technology |
|------|-------------|
| Engine | [Godot 4.5 .NET](https://godotengine.org) |
| Language | C# 13 / .NET 9 |
| Data Format | JSON |
| Version Control | Git + GitHub |
| CI / Export | GitHub Actions |
| License | Code → MIT, Assets → CC BY-NC-SA 4.0 |

---

## 📁 Project Structure

```
pixels-refactory/
├── .github/
│   ├── workflows/
│   │   ├── ci.yml
│   │   └── export.yml
│   └── copilot-instructions.md
├── Game/
│   ├── Scenes/                 # GraphScene, Nodes, HUD
│   ├── Scripts/
│   │   ├── Simulation/         # Core simulation logic
│   │   │   ├── Core/
│   │   │   ├── Nodes/
│   │   │   └── Systems/
│   │   ├── GraphEditor/        # GraphEdit UI and node creation
│   │   └── UI/                 # Play/Pause controls, KPIs
│   ├── Data/
│   │   └── sample_graph.json   # Demo graph for first run
│   └── Assets/                 # Art, music, icons
├── LICENSE
├── project.godot
├── export_presets.cfg
├── README.md
└── .editorconfig (optional)
```

---

## 🚀 Build & Run

### 📋 Prerequisites
- **Godot 4.5 .NET** (Mono) — [Download here](https://godotengine.org/download)
- **.NET 9 SDK** — [Download here](https://dotnet.microsoft.com/download)

### 🔨 Build locally
```bash
godot --headless --build-solutions --quit
dotnet restore
dotnet build --configuration Release
```

### 🎮 Run in Editor
Open `project.godot` in Godot → press ▶ to start the simulation.

### 🎯 Using the Graph Editor
1. **Create Nodes**: Click toolbar buttons (`+ Event`, `+ Function`, `+ Gateway`, `+ Buffer`, `+ Sink`)
2. **Connect Nodes**: Drag from output ports (right) to input ports (left)
3. **Run Simulation**: Click `▶ Play` button in the control panel
4. **Adjust Speed**: Click speed button to cycle through 1x → 2x → 5x
5. **Monitor KPIs**: View metrics in the right panel (items consumed, quality, throughput)

For detailed usage instructions, see [Game/Scenes/README.md](Game/Scenes/README.md).  
For architecture details, see [EDITOR_GUIDE.md](EDITOR_GUIDE.md).

### 🧪 Optional: Headless smoke test
```bash
godot --headless --quit
```

---

## ⚙️ Continuous Integration
| Workflow | Trigger | Purpose |
|-----------|----------|----------|
| ci.yml | Push / PR | Build validation |
| export.yml | Tag (v*) | Windows & Linux exports |

---

## 📦 Creating a Release

To create a new release and automatically build Windows/Linux exports:

1. **Ensure CI is passing** on your main branch
2. **Create and push a version tag:**
   ```bash
   git tag v0.1.0
   git push origin v0.1.0
   ```
3. The `export.yml` workflow will automatically:
   - Build the game for Windows and Linux
   - Package the builds as zip/tar.gz archives
   - Create a GitHub release with the artifacts attached

**Tag naming convention:** Use semantic versioning (e.g., `v0.1.0`, `v1.0.0`, `v1.2.3-beta`)
- Tags **without** a hyphen (e.g., `v1.0.0`) → standard release
- Tags **with** a hyphen (e.g., `v1.0.0-beta`, `v1.2.3-rc1`) → pre-release

---

## 🤝 Contributing
1. Fork the repo  
2. Create a feature branch  
3. Run `dotnet build`  
4. Submit a PR  

All contributions must pass CI and follow `.github/copilot-instructions.md`.

### Code Formatting
This project uses `.editorconfig` for consistent code formatting:
- **C# files**: Use **tabs** for indentation (tab size = 4)
- **GDScript files**: Use **spaces** for indentation (4 spaces)
- **JSON/YAML files**: Use **spaces** for indentation (2 spaces)

**Godot Editor** automatically respects `.editorconfig` settings in Godot 4.5+.  
When you create or edit scripts in the Godot editor, it will apply the correct indentation style.

**Visual Studio Code** and other modern editors also support `.editorconfig` natively.

---

## 📄 License
**Code:** MIT  
**Assets:** CC BY-NC-SA 4.0  
Credit required, non-commercial use only for assets.

---

## 🗺️ Roadmap
- [ ] Node editor MVP  
- [ ] JSON graph import/export  
- [ ] Tick simulation + KPIs  
- [ ] Pixel mini-game prototype  
- [ ] Kusintel AI tree  

> **Note:** Web deployment is not available for Godot 4.x with C#/.NET due to upstream limitations. Native Windows and Linux exports are supported.  

---

> _"Every tick a beat. Every commit a note."_ — **Pixel** 🕷️✨
