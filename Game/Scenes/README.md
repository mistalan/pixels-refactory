# Node-Graph Factory Editor

## Overview

The Node-Graph Factory Editor is an EPK-style (Event-driven Process Chain) flowchart editor for designing production pipelines. It allows you to visually create and connect nodes representing different stages of a software production process.

## Features

- **Visual Graph Editor**: Drag and drop interface using Godot's GraphEdit
- **Multiple Node Types**: Event, Function, Gateway, Buffer, and Sink nodes
- **Tick-Based Simulation**: Deterministic, discrete-tick simulation (10 ticks/second default)
- **Acyclic Validation**: Automatic cycle detection to ensure valid DAG (Directed Acyclic Graph)
- **Real-time KPIs**: Track items in transit, throughput, and quality metrics
- **Speed Control**: Run at 1x, 2x, or 5x speed

## Node Types

### Event Node
- **Purpose**: Entry point for items into the factory
- **Behavior**: Generates items at a specified rate
- **Configuration**:
  - `ItemType`: Type of item to generate (Ticket, Spec, Code, Test, Build, Doc)
  - `GenerationRate`: Items generated per tick
  - `Quality`: Initial quality (0.0 to 1.0+)
- **Ports**: 0 inputs, 1 output

### Function Node
- **Purpose**: Processes items with throughput and latency
- **Behavior**: Transforms items, can change type and quality
- **Configuration**:
  - `Throughput`: Max items processed per tick
  - `Latency`: Ticks items wait before processing
  - `OutputType`: Transform items to this type (optional)
  - `QualityMultiplier`: Quality adjustment (1.0 = no change)
- **Ports**: 1 input, 1 output

### Gateway Node
- **Purpose**: Routes items based on conditions (EPK-style)
- **Types**:
  - `XOR`: Route based on quality threshold (high quality → port 0, low → port 1)
  - `AND`: Duplicate items to all outputs
  - `OR`: Route to first available output
- **Ports**: 1 input, 2 outputs

### Buffer Node
- **Purpose**: Temporary storage for load balancing
- **Behavior**: Stores items in internal queue, releases at controlled rate
- **Configuration**:
  - `Capacity`: Maximum items to store
  - `ReleaseRate`: Items released per tick
- **Ports**: 1 input, 1 output

### Sink Node
- **Purpose**: Terminal node that consumes items and tracks metrics
- **Behavior**: Accepts all incoming items and calculates KPIs
- **Metrics**:
  - Total items consumed
  - Average quality
  - Last tick throughput
- **Ports**: 1 input, 0 outputs

## How to Use

### Creating Nodes
1. Click one of the toolbar buttons at the top:
   - `+ Event` - Create an event generator
   - `+ Function` - Create a processing node
   - `+ Gateway` - Create a routing node
   - `+ Buffer` - Create a storage node
   - `+ Sink` - Create a terminal node
2. Nodes are automatically placed in sequence from left to right

### Connecting Nodes
1. Click and drag from an output port (right side) to an input port (left side)
2. Connections are validated automatically
3. If a connection would create a cycle, it will be rejected with an error message

### Disconnecting Nodes
1. Right-click on a connection line
2. Or use the disconnect tool in GraphEdit

### Deleting Nodes
1. Select nodes by clicking them
2. Press the Delete/Backspace key
3. Or right-click and choose delete

### Running the Simulation
1. **Play**: Click the `▶ Play` button to start the simulation
2. **Pause**: Click the `⏸ Pause` button to pause
3. **Speed**: Click the speed button to cycle through 1x → 2x → 5x speeds
4. **Status**: Monitor tick count and status in the control panel

### Monitoring KPIs
The KPI panel (right side) shows:
- Total nodes and edges in the graph
- Items currently in transit
- Per-sink metrics (total consumed, average quality, throughput)

## Graph Validation

The editor automatically validates:
- **Acyclic Graph**: No cycles are allowed (DAG requirement)
- **Connected Edges**: All edges must connect to existing nodes
- **Warnings**: Nodes without outputs (except sinks) generate warnings

## Simulation Details

### Tick System
- Default: 10 ticks per second
- Each tick processes all nodes in topological order
- Ensures deterministic behavior

### Item Flow
- Items flow through edges as FIFO queues
- Each edge maintains its own queue
- Nodes pull from input edges and push to output edges

### Quality Tracking
- Items have a quality metric (0.0 to 1.0+)
- Function nodes can modify quality via multipliers
- Sinks track average quality over time

## Example Pipeline

A simple software development pipeline:

```
[Event: Ticket] → [Function: Code] → [Function: Test] → [Sink]
```

1. **Event**: Generates ticket items
2. **Code Function**: Transforms tickets to code (with latency)
3. **Test Function**: Transforms code to tests
4. **Sink**: Consumes completed tests and tracks metrics

## Keyboard Shortcuts

- `Delete/Backspace`: Delete selected nodes
- `Mouse Wheel`: Zoom in/out
- `Middle Mouse Drag`: Pan the graph
- `Right Click`: Context menu / disconnect

## Tips

- Start with Event nodes on the left and Sinks on the right
- Use Buffers to handle throughput mismatches
- Gateways are useful for quality-based routing
- Monitor the KPI panel to optimize your pipeline
- Adjust speed to observe behavior at different rates

## Technical Notes

- Built on Godot 4.5 .NET with C# 13
- Uses Godot's GraphEdit and GraphNode components
- Implements EPK (Event-driven Process Chain) patterns
- All simulation code is in `Game/Scripts/Simulation/`
- Graph editor UI code is in `Game/Scripts/GraphEditor/`
