# Quick Start Guide - Node-Graph Factory Editor

## 🚀 Get Started in 60 Seconds

### Prerequisites
1. Install **Godot 4.5 .NET** from [godotengine.org](https://godotengine.org/download)
2. Install **.NET 9 SDK** from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

### Open the Project
```bash
# Clone the repository
git clone https://github.com/mistalan/pixels-refactory.git
cd pixels-refactory

# Build C# bindings
godot --headless --build-solutions --quit

# Open in Godot Editor
godot project.godot
```

## 🎮 Your First Pipeline

### Step 1: Create Nodes (10 seconds)
Click these buttons in order:
1. `+ Event` (creates a ticket generator)
2. `+ Function` (creates a code processor)
3. `+ Sink` (creates an output collector)

### Step 2: Connect Nodes (10 seconds)
1. Drag from the **green output** of Event node
2. To the **blue input** of Function node
3. Drag from the **blue output** of Function node
4. To the **red input** of Sink node

### Step 3: Run Simulation (5 seconds)
1. Click `▶ Play` button at the bottom
2. Watch items flow through your pipeline!
3. Check the **KPI Panel** on the right for metrics

### Step 4: Experiment (35 seconds)
- Click the **Speed** button to go faster (2x, 5x)
- Add more nodes (try `+ Buffer` or `+ Gateway`)
- Create parallel paths
- Watch the KPIs change in real-time

## 🎯 Node Reference

| Node | Symbol | Purpose | Ports |
|------|--------|---------|-------|
| **Event** | 🟢 | Generates items | → Out |
| **Function** | 🔵 | Processes items | In → Out |
| **Gateway** | 🟡 | Routes items | In → Out1, Out2 |
| **Buffer** | 🔵 | Stores items | In → Out |
| **Sink** | 🔴 | Consumes items | In → |

## 📊 Reading KPIs

The **KPI Panel** (top right) shows:
- **Nodes**: Total nodes in your graph
- **Edges**: Total connections
- **Items in transit**: Items currently flowing
- **Sink metrics**: Per sink:
  - Total: Items consumed
  - Avg Quality: 0.0 (poor) to 1.0+ (excellent)
  - Last Tick: Items consumed in last tick

## ⚡ Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Delete` | Delete selected nodes |
| `Mouse Wheel` | Zoom in/out |
| `Middle Mouse` | Pan the graph |
| `Right Click` | Disconnect connection |

## 💡 Pro Tips

### Tip 1: Start Simple
Begin with Event → Function → Sink, then expand.

### Tip 2: Use Buffers
If a Function node can't keep up, add a Buffer before it.

### Tip 3: Watch for Cycles
The editor will reject connections that create cycles (red error in console).

### Tip 4: Slow Motion
Use 1x speed to debug complex flows.

### Tip 5: Quality Tracking
Function nodes can improve or degrade quality. Check the Sink's Avg Quality.

## 🔧 Common Patterns

### Pattern: Assembly Line
```
Event → Function → Function → Sink
```
Classic linear pipeline for sequential processing.

### Pattern: Parallel Processing
```
      ┌→ Function1 → Sink1
Event─┤
      └→ Function2 → Sink2
```
Use multiple outputs to split work (requires Gateway AND).

### Pattern: Buffer Zone
```
Event → Buffer → Function → Sink
```
Smooth out throughput mismatches.

## 🐛 Troubleshooting

### Problem: "Graph contains cycles"
**Solution**: You created a loop. Remove the last connection that created the cycle.

### Problem: No items flowing
**Solution**: 
1. Check if simulation is playing (▶ symbol should show "Running")
2. Verify all nodes are connected
3. Ensure Event node is generating (check GenerationRate)

### Problem: Items stuck
**Solution**:
1. Check Function node throughput (might be too low)
2. Check Buffer capacity (might be full)
3. Verify connections are correct (drag from right to left side of nodes)

### Problem: Can't connect nodes
**Solution**:
1. Drag from OUTPUT (right side) to INPUT (left side)
2. Check if nodes are compatible (Sink has no outputs)
3. Ensure you're not creating a cycle

## 📚 Learn More

- **User Guide**: `Game/Scenes/README.md` - Detailed feature explanations
- **Developer Guide**: `EDITOR_GUIDE.md` - Architecture and extending
- **Feature List**: `FEATURES.md` - Complete feature reference
- **Architecture**: `ARCHITECTURE.md` - System diagrams

## 🎓 5-Minute Tutorial

### Minute 1: Create a Basic Pipeline
1. Create: Event → Function → Sink
2. Connect them in order
3. Click Play

### Minute 2: Add Complexity
1. Click Pause
2. Add another Function between existing ones
3. Reconnect: Event → Function → Function → Function → Sink
4. Click Play

### Minute 3: Monitor Performance
1. Watch the KPI panel
2. Change speed to 5x
3. Observe items per tick changing

### Minute 4: Add a Buffer
1. Click Pause
2. Add Buffer between any two nodes
3. Reconnect through the Buffer
4. Click Play and compare KPIs

### Minute 5: Experiment
1. Delete a node (select it, press Delete)
2. Add a Gateway node
3. Try to create a cycle (spoiler: editor will block it!)
4. Create your own custom pipeline

## 🎉 Next Steps

Once comfortable with the basics:
1. Try all 5 node types
2. Build a complete software pipeline: Ticket → Code → Test → Build → Deploy → Sink
3. Optimize for throughput (items per tick)
4. Optimize for quality (avg quality metric)
5. Design your own production workflow

## 💬 Need Help?

- Check the console (Godot Output panel) for error messages
- Review the detailed guides in the docs folder
- All validation errors include helpful messages
- The editor prevents most invalid operations automatically

---

**Time to First Pipeline**: ~30 seconds  
**Time to Understanding**: ~5 minutes  
**Time to Mastery**: ~30 minutes  

Happy building! 🏭✨
