# Node-Graph Factory Editor - Developer Guide

## Architecture Overview

The Node-Graph Factory Editor is a tick-based, event-driven production line simulator built on Godot 4.5 .NET. It follows EPK (Event-driven Process Chain) patterns for process modeling.

## Key Components

### 1. Simulation Layer (`Game/Scripts/Simulation/`)

#### Core Components (`Core/`)
- **Item.cs**: Data class representing items flowing through the graph
  - `ItemType`: Enum for Ticket, Spec, Code, Test, Build, Doc
  - `Quality`: Float metric (0.0 to 1.0+)
  - `Size`: Integer complexity metric

- **INodeLogic.cs**: Interface for all simulation nodes
  - `Tick(SimContext)`: Process one simulation tick
  - Implemented by all node types

- **Edge.cs**: Directed connection between nodes
  - Contains FIFO queue for items in transit
  - Tracks source/destination nodes and ports

- **SimContext.cs**: Central world state container
  - Registry of all nodes and edges
  - Tick counter and simulation state
  - Helper methods for pushing/pulling items

#### Node Types (`Nodes/`)
All nodes implement `INodeLogic` and process items during their tick:

- **EventNode**: Generates items at specified rate (entry point)
- **FunctionNode**: Processes items with throughput/latency, can transform types
- **GatewayNode**: Routes items (XOR/AND/OR logic)
- **BufferNode**: Temporary storage with release rate control
- **SinkNode**: Consumes items and tracks KPIs (terminal node)

#### Systems (`Systems/`)
- **TopologicalSort.cs**: Kahn's algorithm for node ordering
  - Ensures deterministic execution order
  - Detects cycles (returns null if cycle found)

- **GraphValidator.cs**: Validates graph structure
  - Checks for cycles (graphs must be DAG)
  - Validates edge connections
  - Generates warnings for potential issues

- **Scheduler.cs**: Tick execution manager
  - Calculates execution order via topological sort
  - Executes nodes in dependency order each tick
  - Updates tick counter

### 2. Graph Editor Layer (`Game/Scripts/GraphEditor/`)

#### GraphEditorController.cs
Main controller bridging Godot's GraphEdit to simulation:
- Creates and manages visual GraphNodes
- Handles connection/disconnection requests
- Validates graph structure on changes
- Provides access to SimContext

Key responsibilities:
- Node creation (Event, Function, Gateway, Buffer, Sink)
- Visual representation via GraphNode
- Port configuration (input/output slots)
- Edge management and validation

#### GraphSceneController.cs
Top-level scene controller:
- Wires together GraphEditor, SimulationController, and KPIPanel
- Handles toolbar button presses
- Manages node positioning

### 3. UI Layer (`Game/Scripts/UI/`)

#### SimulationController.cs
Playback controls:
- Play/Pause buttons
- Speed multiplier (1x, 2x, 5x)
- Tick timer and execution
- Status display

Process flow:
1. User presses Play
2. Initializes Scheduler (calculates topology)
3. Each frame, accumulates delta time
4. When timer exceeds tick interval, executes tick
5. Updates status label

#### KPIPanel.cs
Metrics display:
- Node and edge counts
- Items in transit across all edges
- Per-sink metrics (total consumed, avg quality, throughput)
- Updates every 0.5 seconds

## Data Flow

### Item Creation and Flow
```
1. EventNode.Tick() creates Item
2. Calls ctx.PushToOutputs(nodeId, item, port)
3. SimContext finds matching edges
4. Enqueues item in edge.Queue
5. FunctionNode.Tick() calls ctx.PullFromInputs(nodeId, count)
6. SimContext dequeues from incoming edges
7. FunctionNode processes and pushes to outputs
8. Repeat until SinkNode consumes
```

### Tick Execution
```
1. User clicks Play
2. SimulationController.OnPlayPressed()
3. Calls GraphEditor.InitializeScheduler()
4. Scheduler calculates topological order
5. Each frame:
   - Accumulate time with speed multiplier
   - While time >= tick interval:
     - Call Scheduler.Tick()
     - Scheduler executes each node in order
     - Each node's Tick() pulls, processes, pushes
     - Increment tick counter
```

### Graph Validation
```
1. User connects nodes (GraphEditor.OnConnectionRequest)
2. Create Edge in SimContext
3. Call GraphValidator.Validate(ctx)
4. Validator checks:
   - Cycles via TopologicalSort
   - Orphaned edges
   - Nodes without outputs
5. If cycle detected:
   - Remove edge
   - Disconnect visual connection
   - Print error
```

## Scene Structure

```
GraphScene (Control) [GraphSceneController]
├── Toolbar (Panel)
│   └── ToolbarButtons (HBoxContainer)
│       ├── CreateEventButton
│       ├── CreateFunctionButton
│       ├── CreateGatewayButton
│       ├── CreateBufferButton
│       └── CreateSinkButton
├── GraphEditor (GraphEdit) [GraphEditorController]
│   └── (Dynamic GraphNodes created at runtime)
├── ControlPanel (Panel)
│   └── SimulationController (HBoxContainer)
│       ├── PlayButton
│       ├── PauseButton
│       ├── SpeedButton
│       └── StatusLabel
└── KPIPanel (Panel)
    └── KPILabel
```

## Extending the Editor

### Adding a New Node Type

1. **Create Node Class** (`Game/Scripts/Simulation/Nodes/YourNode.cs`):
```csharp
public sealed class YourNode : INodeLogic
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = "Your Node";
    
    public void Tick(SimContext ctx)
    {
        // Your logic here
    }
}
```

2. **Update GraphEditorController.CreateNode()**:
```csharp
case "yournode":
    logic = new YourNode(nodeId);
    break;
```

3. **Add Toolbar Button** (in GraphScene.tscn or code)

4. **Update CreateVisualNode()** for custom port configuration

### Adding New Item Properties

Extend `Item.cs` with additional fields and update node logic to process them.

### Custom Validation Rules

Add checks in `GraphValidator.Validate()` method.

### Additional KPIs

Extend `SinkNode` or create custom tracking in `SimContext`.

## Performance Considerations

- **Topological Sort**: O(V + E) per graph change
- **Tick Execution**: O(V) per tick (each node processed once)
- **Item Flow**: O(E) per tick (checking edges for items)
- **Current Scalability**: Designed for ~100 nodes, ~200 edges
- **Optimization Opportunities**:
  - Cache execution order until graph changes
  - Batch item processing
  - Spatial partitioning for large graphs

## Testing

Currently manual testing via:
1. Create nodes using toolbar
2. Connect nodes
3. Press Play
4. Monitor KPIs

Future: Add unit tests for:
- Topological sort edge cases
- Node logic (throughput, quality calculations)
- Graph validation rules
- Scheduler execution order

## Debug Tips

- Use `GD.Print()` for logging
- Check Godot console for validation errors
- Monitor KPI panel for unexpected item counts
- Verify execution order via Scheduler.GetExecutionOrder()
- Test with speed = 1x first, then increase

## Known Limitations

1. **Latency Implementation**: Simplified in FunctionNode (items not queued internally)
2. **Port Limits**: Gateways hardcoded to 2 outputs
3. **No Serialization**: Graphs created at runtime not saved yet
4. **No JSON Import**: sample_graph.json structure defined but import not implemented
5. **No Visual Feedback**: Items not rendered on edges (future feature)

## Next Steps

Priority features for V2:
- [ ] Graph save/load (JSON serialization)
- [ ] Visual item flow (particles/sprites on edges)
- [ ] Node configuration UI (edit throughput, latency at runtime)
- [ ] Undo/redo support
- [ ] Graph templates
- [ ] Performance profiler overlay
