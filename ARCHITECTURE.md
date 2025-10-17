# Node-Graph Factory Editor - Architecture Diagram

## System Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                         GraphScene.tscn                              │
│                     (Main Scene - Control)                           │
└─────────────────────────────────────────────────────────────────────┘
                                  │
                    ┌─────────────┴─────────────┐
                    │   GraphSceneController    │
                    │   (Wires everything up)   │
                    └─────────────┬─────────────┘
                                  │
          ┌───────────────────────┼───────────────────────┐
          │                       │                       │
          ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  GraphEditor    │    │  Simulation     │    │   KPIPanel      │
│  (GraphEdit)    │    │  Controller     │    │  (Metrics UI)   │
│                 │    │  (Play/Pause)   │    │                 │
└────────┬────────┘    └────────┬────────┘    └────────┬────────┘
         │                      │                       │
         │                      │                       │
         ▼                      ▼                       │
┌─────────────────┐    ┌─────────────────┐            │
│ GraphEditor     │◄───│   Scheduler     │            │
│ Controller      │    │  (Tick Exec)    │            │
│                 │    └────────┬────────┘            │
└────────┬────────┘             │                      │
         │                      │                      │
         │                      ▼                      │
         │             ┌─────────────────┐            │
         └────────────►│   SimContext    │◄───────────┘
                       │  (World State)  │
                       └────────┬────────┘
                                │
                  ┌─────────────┼─────────────┐
                  │                           │
                  ▼                           ▼
         ┌─────────────────┐         ┌─────────────────┐
         │  Nodes Registry │         │  Edges Registry │
         │  (INodeLogic)   │         │  (Item Queues)  │
         └─────────────────┘         └─────────────────┘
```

## Layer Architecture

```
┌─────────────────────────────────────────────────────────┐
│                      UI Layer                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │GraphScene    │  │SimController │  │   KPIPanel   │  │
│  │Controller    │  │              │  │              │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                   Graph Editor Layer                     │
│  ┌──────────────────────────────────────────────────┐   │
│  │         GraphEditorController                    │   │
│  │  - Creates visual GraphNodes                     │   │
│  │  - Handles connections                           │   │
│  │  - Validates graph structure                     │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                  Simulation Layer                        │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐        │
│  │ SimContext │  │ Scheduler  │  │ Validator  │        │
│  └────────────┘  └────────────┘  └────────────┘        │
│                                                          │
│  ┌──────────────────────────────────────────────────┐   │
│  │              Node Types                          │   │
│  │  Event  Function  Gateway  Buffer  Sink         │   │
│  └──────────────────────────────────────────────────┘   │
│                                                          │
│  ┌──────────────────────────────────────────────────┐   │
│  │              Core Data                           │   │
│  │  Item  Edge  INodeLogic                         │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

## Data Flow - Node Creation

```
User Clicks               GraphScene              GraphEditor
"+ Event"                 Controller              Controller
    │                          │                        │
    └──────Pressed─────────────►                        │
                               │                        │
                               └──CreateNode("event")───►
                                                         │
                                                    ┌────▼─────┐
                                                    │ Create   │
                                                    │ EventNode│
                                                    │ logic    │
                                                    └────┬─────┘
                                                         │
                                                    ┌────▼─────┐
                                                    │ Add to   │
                                                    │SimContext│
                                                    └────┬─────┘
                                                         │
                                                    ┌────▼─────┐
                                                    │ Create   │
                                                    │GraphNode │
                                                    │ visual   │
                                                    └──────────┘
```

## Data Flow - Tick Execution

```
Timer Tick               SimController           Scheduler           Nodes
    │                          │                     │                 │
    ├───Process(delta)────────►│                     │                 │
    │                          │                     │                 │
    │                      ┌───▼───┐                 │                 │
    │                      │Accum  │                 │                 │
    │                      │Timer  │                 │                 │
    │                      └───┬───┘                 │                 │
    │                          │                     │                 │
    │                    Timer >= Interval?          │                 │
    │                          │                     │                 │
    │                          └──────Tick()────────►│                 │
    │                                                 │                 │
    │                                            ┌────▼────┐            │
    │                                            │Get Topo │            │
    │                                            │Order    │            │
    │                                            └────┬────┘            │
    │                                                 │                 │
    │                                            For each node:         │
    │                                                 │                 │
    │                                                 └────Tick()──────►│
    │                                                                   │
    │                                                              ┌────▼────┐
    │                                                              │Pull     │
    │                                                              │Items    │
    │                                                              └────┬────┘
    │                                                                   │
    │                                                              ┌────▼────┐
    │                                                              │Process  │
    │                                                              │Logic    │
    │                                                              └────┬────┘
    │                                                                   │
    │                                                              ┌────▼────┐
    │                                                              │Push     │
    │                                                              │Outputs  │
    │                                                              └─────────┘
```

## Data Flow - Item Journey

```
EventNode           Edge1           FunctionNode        Edge2           SinkNode
    │                │                   │                │                 │
    │─New Item──────►│                   │                │                 │
    │                │                   │                │                 │
    │                │◄──PullFromInputs──│                │                 │
    │                │                   │                │                 │
    │                │───Dequeue Item───►│                │                 │
    │                │                   │                │                 │
    │                │              ┌────▼────┐           │                 │
    │                │              │Transform│           │                 │
    │                │              │Quality  │           │                 │
    │                │              └────┬────┘           │                 │
    │                │                   │                │                 │
    │                │                   └─PushToOutputs─►│                 │
    │                │                                    │                 │
    │                │                                    │◄PullFromInputs──│
    │                │                                    │                 │
    │                │                                    │──Dequeue Item──►│
    │                │                                    │                 │
    │                │                                    │            ┌────▼────┐
    │                │                                    │            │Track    │
    │                │                                    │            │KPIs     │
    │                │                                    │            └─────────┘
```

## Component Relationships

```
┌──────────────────────┐
│   INodeLogic         │◄─────────────┬────────────┬──────────┬────────┬─────┐
│   (Interface)        │              │            │          │        │     │
└──────────────────────┘              │            │          │        │     │
                                      │            │          │        │     │
        ┌─────────────────────────────┴┐  ┌────────▼───┐  ┌──▼─────┐ │     │
        │      EventNode              │  │FunctionNode│  │Gateway │ │     │
        │  - GenerationRate           │  │- Throughput│  │Node    │ │     │
        │  - ItemType                 │  │- Latency   │  │- Type  │ │     │
        │  - Quality                  │  │- Transform │  └────────┘ │     │
        └─────────────────────────────┘  └────────────┘             │     │
                                                              ┌──────▼──┐  │
                                                              │Buffer   │  │
                                                              │Node     │  │
                                                              └─────────┘  │
                                                                      ┌────▼───┐
                                                                      │Sink    │
                                                                      │Node    │
                                                                      └────────┘

┌──────────────────────┐            ┌──────────────────────┐
│   SimContext         │◄───────────│   Scheduler          │
│   - Nodes: Dict      │            │   - ExecutionOrder   │
│   - Edges: Dict      │            │   - Tick()           │
│   - TickCount        │            └──────────────────────┘
│   - IsRunning        │
└──────────────────────┘
          │
          │ Contains
          │
    ┌─────┴─────┐
    │           │
    ▼           ▼
┌────────┐  ┌────────┐
│ Nodes  │  │ Edges  │
│Registry│  │Registry│
└────────┘  └────────┘
              │ Contains
              ▼
          ┌──────┐
          │Queue │
          │<Item>│
          └──────┘
```

## File Organization

```
Game/
├── Scenes/
│   ├── GraphScene.tscn           # Main scene file
│   ├── README.md                 # User guide
│   └── Nodes/                    # (Future: node prefabs)
│
├── Scripts/
│   ├── GraphEditor/
│   │   ├── GraphEditorController.cs    # GraphEdit bridge
│   │   └── GraphSceneController.cs     # Scene wiring
│   │
│   ├── Simulation/
│   │   ├── Core/
│   │   │   ├── Item.cs                 # Item data class
│   │   │   ├── INodeLogic.cs           # Node interface
│   │   │   ├── Edge.cs                 # Connection + queue
│   │   │   └── SimContext.cs           # World state
│   │   │
│   │   ├── Nodes/
│   │   │   ├── EventNode.cs            # Item generator
│   │   │   ├── FunctionNode.cs         # Processor
│   │   │   ├── GatewayNode.cs          # Router
│   │   │   ├── BufferNode.cs           # Storage
│   │   │   └── SinkNode.cs             # Consumer
│   │   │
│   │   └── Systems/
│   │       ├── TopologicalSort.cs      # Kahn's algorithm
│   │       ├── GraphValidator.cs       # Validation rules
│   │       └── Scheduler.cs            # Tick execution
│   │
│   └── UI/
│       ├── SimulationController.cs     # Play/pause/speed
│       └── KPIPanel.cs                 # Metrics display
│
└── Data/
    └── sample_graph.json               # Example graph
```

## Key Design Patterns

### 1. **Tick-Based Simulation**
- Fixed timestep (10 ticks/second default)
- Deterministic execution order
- No inter-tick state dependencies

### 2. **Topological Ordering**
- DAG requirement (no cycles)
- Nodes processed in dependency order
- Invalidate on graph structure change

### 3. **FIFO Queues**
- Items flow through edge queues
- Preserves ordering
- Decouples node processing

### 4. **Interface-Based Polymorphism**
- `INodeLogic` abstracts node types
- Common `Tick(SimContext)` contract
- Easy to extend with new node types

### 5. **Central State Manager**
- `SimContext` owns all mutable state
- Nodes are stateless (except internal queues)
- Simplifies testing and debugging

## Extension Points

### Adding New Node Types
1. Implement `INodeLogic`
2. Add to `GraphEditorController.CreateNode()`
3. Configure ports in `CreateVisualNode()`

### Custom Validation Rules
Extend `GraphValidator.Validate()`

### New Item Properties
Extend `Item` class, update node logic

### Serialization
Add JSON readers/writers in new `Serialization/` folder

### Visual Feedback
Add particle systems or sprites to edges in `GraphEditor/`
