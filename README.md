# Pixel's Refactory
### _Automate Creativity._

![Godot](https://img.shields.io/badge/Godot-4.3%20.NET-blue?logo=godot-engine&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)
![Build](https://github.com/<user>/pixels-refactory/actions/workflows/ci.yml/badge.svg)
![Export](https://github.com/<user>/pixels-refactory/actions/workflows/export.yml/badge.svg)

---

## About the Game

> Build, connect, and optimize the world's first creative factory.  
> Automate coding, testing and music production â€” but be careful:  
> every refactor changes the rhythm of the world.

**Pixel's Refactory** is a 2D factory-simulation game built with **Godot 4.3 (.NET)**.  
You design pipelines of **software processes** â€” Events, Functions, Tests, and Deployments â€”  
and feed them into a living world where code and creativity intertwine.

Your tiny companion **Pixel**, a curious digital spider, rewards precision and focus  
through playful mini-games and insights into the art of automation.  
Research new technologies like **Kusintel**, an AI-assistant that helps you  
refactor both code and reality.

---

## Features
- Node-Graph Factory Editor â€“ design event-driven production lines in an EPK-style flowchart  
- Tick-Based Simulation â€“ deterministic throughput, latency, and quality metrics  
- Music Integration â€“ code meets rhythm; automation becomes art  
- AI Research Tree â€“ unlock Kusintel for smarter automation  
- Pixel Mini-Games â€“ gain focus buffs and productivity boosts  
- Data-Driven JSON System â€“ create your own nodes, edges and item types

---

## Tech Stack
| Area | Technology |
|------|-------------|
| Engine | [Godot 4.3 .NET](https://godotengine.org) |
| Language | C# 11 / .NET 8 |
| Data Format | JSON |
| Version Control | Git + GitHub |
| CI / Export | GitHub Actions |
| License | Code â†’ MIT, Assets â†’ CC BY-NC-SA 4.0 |

---

## Project Structure

```
pixels-refactory/
â”œâ”€â”€ .github/
â”‚   â”œâ”€â”€ workflows/
â”‚   â”‚   â”œâ”€â”€ ci.yml
â”‚   â”‚   â”œâ”€â”€ export.yml
â”‚   â”‚   â””â”€â”€ web-deploy.yml
â”‚   â””â”€â”€ copilot-instructions.md
â”œâ”€â”€ Game/
â”‚   â”œâ”€â”€ Scenes/                 # GraphScene, Nodes, HUD
â”‚   â”œâ”€â”€ Scripts/
â”‚   â”‚   â”œâ”€â”€ Simulation/         # Core simulation logic
â”‚   â”‚   â”‚   â”œâ”€â”€ Core/
â”‚   â”‚   â”‚   â”œâ”€â”€ Nodes/
â”‚   â”‚   â”‚   â””â”€â”€ Systems/
â”‚   â”‚   â”œâ”€â”€ GraphEditor/        # GraphEdit UI and node creation
â”‚   â”‚   â””â”€â”€ UI/                 # Play/Pause controls, KPIs
â”‚   â”œâ”€â”€ Data/
â”‚   â”‚   â””â”€â”€ sample_graph.json   # Demo graph for first run
â”‚   â””â”€â”€ Assets/                 # Art, music, icons
â”œâ”€â”€ LICENSE
â”œâ”€â”€ project.godot
â”œâ”€â”€ export_presets.cfg
â”œâ”€â”€ README.md
â””â”€â”€ .editorconfig (optional)
```

---

## Build & Run

### Prerequisites
- Godot 4.3 .NET (Mono)
- .NET 8 SDK

### Build locally
```bash
godot --headless --build-solutions --quit
dotnet restore
dotnet build --configuration Release
```

### Run in Editor
Open `project.godot` in Godot â†’ press â–¶ to start the simulation.

### Optional: Headless smoke test
```bash
godot --headless --quit
```

---

## Continuous Integration
| Workflow | Trigger | Purpose |
|-----------|----------|----------|
| ci.yml | Push / PR | Build validation |
| export.yml | Tag (v*) | Cross-platform exports |
| web-deploy.yml | Main branch | Deploys Web build to GitHub Pages |

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
- [ ] Web demo on GitHub Pages  

---

"Every tick a beat. Every commit a note." â€” Pixel
