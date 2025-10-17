using Godot;
using PixelsRefactory.Simulation.Core;
using PixelsRefactory.Simulation.Nodes;
using PixelsRefactory.Simulation.Systems;
using System;
using System.Collections.Generic;

namespace PixelsRefactory.GraphEditor;

/// <summary>
/// Main controller for the graph editor.
/// Manages GraphEdit UI and bridges to simulation layer.
/// </summary>
public partial class GraphEditorController : GraphEdit
{
    private SimContext _simContext = new();
    private Scheduler? _scheduler;
    private readonly Dictionary<string, GraphNode> _visualNodes = new();
    private int _nodeIdCounter = 0;

    public override void _Ready()
    {
        // Configure GraphEdit
        RightDisconnects = true;
        ShowZoomLabel = true;
        
        // Connect signals
        ConnectionRequest += OnConnectionRequest;
        DisconnectionRequest += OnDisconnectionRequest;
        DeleteNodesRequest += OnDeleteNodesRequest;
        
        GD.Print("GraphEditorController ready");
    }

    /// <summary>
    /// Create a new node in the graph.
    /// </summary>
    public void CreateNode(string nodeType, Vector2 position)
    {
        string nodeId = $"node_{_nodeIdCounter++}";
        INodeLogic? logic = null;

        // Create the logic node based on type
        switch (nodeType.ToLower())
        {
            case "event":
                logic = new EventNode(nodeId, ItemType.Ticket, 1);
                break;
            case "function":
                logic = new FunctionNode(nodeId, throughput: 1, latency: 0, outputType: ItemType.Code);
                break;
            case "gateway":
                logic = new GatewayNode(nodeId, GatewayType.XOR);
                break;
            case "buffer":
                logic = new BufferNode(nodeId, capacity: 10, releaseRate: 1);
                break;
            case "sink":
                logic = new SinkNode(nodeId);
                break;
            default:
                GD.PrintErr($"Unknown node type: {nodeType}");
                return;
        }

        if (logic != null)
        {
            // Add to simulation context
            _simContext.AddNode(logic);
            
            // Create visual node
            var visualNode = CreateVisualNode(logic, position);
            _visualNodes[nodeId] = visualNode;
            AddChild(visualNode);
            
            GD.Print($"Created {nodeType} node: {nodeId}");
        }
    }

    /// <summary>
    /// Create the visual GraphNode for display.
    /// </summary>
    private GraphNode CreateVisualNode(INodeLogic logic, Vector2 position)
    {
        var node = new GraphNode
        {
            Name = logic.Id,
            Title = logic.DisplayName,
            PositionOffset = position,
            Draggable = true,
            Resizable = false,
            ShowClose = true
        };

        // Add a label showing node info
        var label = new Label
        {
            Text = $"ID: {logic.Id}\nType: {logic.GetType().Name}"
        };
        node.AddChild(label);

        // Set slots based on node type
        if (logic is EventNode)
        {
            node.SetSlot(0, false, 0, Colors.White, true, 0, Colors.Green);
        }
        else if (logic is SinkNode)
        {
            node.SetSlot(0, true, 0, Colors.Red, false, 0, Colors.White);
        }
        else if (logic is GatewayNode)
        {
            // Gateway has 1 input, 2 outputs
            node.SetSlot(0, true, 0, Colors.Yellow, true, 0, Colors.Yellow);
        }
        else
        {
            // Default: 1 input, 1 output
            node.SetSlot(0, true, 0, Colors.Blue, true, 0, Colors.Blue);
        }

        return node;
    }

    /// <summary>
    /// Handle connection request between nodes.
    /// </summary>
    private void OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
    {
        string fromId = fromNode.ToString();
        string toId = toNode.ToString();
        
        // Create edge in simulation
        var edge = new Edge(fromId, toId, (int)fromPort, (int)toPort);
        _simContext.AddEdge(edge);
        
        // Create visual connection
        ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
        
        // Validate graph
        var validation = GraphValidator.Validate(_simContext);
        if (!validation.IsValid)
        {
            GD.PrintErr("Graph validation failed:");
            foreach (var error in validation.Errors)
                GD.PrintErr($"  - {error}");
            
            // Remove the edge if it creates a cycle
            if (validation.Errors.Exists(e => e.Contains("cycle")))
            {
                _simContext.RemoveEdge(fromId, (int)fromPort, toId, (int)toPort);
                DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
                GD.PrintErr("Connection rejected: would create a cycle");
            }
        }
        else
        {
            GD.Print($"Connected {fromId}[{fromPort}] -> {toId}[{toPort}]");
        }
    }

    /// <summary>
    /// Handle disconnection request.
    /// </summary>
    private void OnDisconnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
    {
        string fromId = fromNode.ToString();
        string toId = toNode.ToString();
        
        // Remove edge from simulation
        _simContext.RemoveEdge(fromId, (int)fromPort, toId, (int)toPort);
        
        // Remove visual connection
        DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
        
        GD.Print($"Disconnected {fromId}[{fromPort}] -> {toId}[{toPort}]");
    }

    /// <summary>
    /// Handle delete nodes request.
    /// </summary>
    private void OnDeleteNodesRequest(Array nodes)
    {
        foreach (var node in nodes)
        {
            if (node is StringName nodeName)
            {
                string nodeId = nodeName.ToString();
                
                // Remove from simulation
                _simContext.RemoveNode(nodeId);
                
                // Remove visual node
                if (_visualNodes.TryGetValue(nodeId, out var visualNode))
                {
                    visualNode.QueueFree();
                    _visualNodes.Remove(nodeId);
                }
                
                GD.Print($"Deleted node: {nodeId}");
            }
        }
    }

    /// <summary>
    /// Get the simulation context.
    /// </summary>
    public SimContext GetSimContext()
    {
        return _simContext;
    }

    /// <summary>
    /// Initialize the scheduler.
    /// </summary>
    public void InitializeScheduler()
    {
        _scheduler = new Scheduler(_simContext);
        if (!_scheduler.RecalculateOrder())
        {
            GD.PrintErr("Failed to initialize scheduler: graph contains cycles");
        }
    }

    /// <summary>
    /// Execute one simulation tick.
    /// </summary>
    public void SimulateTick()
    {
        _scheduler?.Tick();
    }
}
