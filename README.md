# Pixel's Refactory
### _Automate Creativity._

![Godot](https://img.shields.io/badge/Godot-4.5%20.NET-blue?logo=godot-engine&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)
![Build](https://github.com/<user>/pixels-refactory/actions/workflows/ci.yml/badge.svg)
![Export](https://github.com/<user>/pixels-refactory/actions/workflows/export.yml/badge.svg)

---

## About the Game

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

## Features
- Node-Graph Factory Editor — design event-driven production lines in an EPK-style flowchart  
- Tick-Based Simulation — deterministic throughput, latency, and quality metrics  
- Music Integration — code meets rhythm; automation becomes art  
- AI Research Tree — unlock Kusintel for smarter automation  
- Pixel Mini-Games — gain focus buffs and productivity boosts  
- Data-Driven JSON System — create your own nodes, edges and item types

---

## Tech Stack
| Area | Technology |
|------|-------------|
| Engine | [Godot 4.5 .NET](https://godotengine.org) |
| Language | C# 11 / .NET 8 |
| Data Format | JSON |
| Version Control | Git + GitHub |
| CI / Export | GitHub Actions |
| License | Code → MIT, Assets → CC BY-NC-SA 4.0 |

---

## Project Structure

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

## Build & Run

### Prerequisites
- Godot 4.5 .NET (Mono)
- .NET 8 SDK

### Build locally
```bash
godot --headless --build-solutions --quit
dotnet restore
dotnet build --configuration Release
```

### Run in Editor
Open `project.godot` in Godot → press ▶ to start the simulation.

### Optional: Headless smoke test
```bash
godot --headless --quit
```

---

## Continuous Integration
| Workflow | Trigger | Purpose |
|-----------|----------|----------|
| ci.yml | Push / PR | Build validation |
| export.yml | Tag (v*) | Windows & Linux exports |

---

## Contributing
1. Fork the repo  
2. Create a feature branch  
3. Run `dotnet build`  
4. Submit a PR  

All contributions must pass CI and follow `.github/copilot-instructions.md`.

---

## License
**Code:** MIT  
**Assets:** CC BY-NC-SA 4.0  
Credit required, non-commercial use only for assets.

---

## Roadmap
- [ ] Node editor MVP  
- [ ] JSON graph import/export  
- [ ] Tick simulation + KPIs  
- [ ] Pixel mini-game prototype  
- [ ] Kusintel AI tree  

> **Note:** Web deployment is not available for Godot 4.x with C#/.NET due to upstream limitations. Native Windows and Linux exports are supported.  

---

"Every tick a beat. Every commit a note." — Pixel
