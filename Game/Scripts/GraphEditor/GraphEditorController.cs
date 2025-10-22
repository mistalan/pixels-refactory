using System;
using System.Collections.Generic;
using Godot;
using PixelsRefactory.Simulation.Core;
using PixelsRefactory.Simulation.Nodes;
using PixelsRefactory.Simulation.Systems;

namespace PixelsRefactory.GraphEditor;

/// <summary>
/// Main controller for the graph editor.
/// Manages GraphEdit UI and bridges to simulation layer.
/// </summary>
public partial class GraphEditorController : GraphEdit
{
	private readonly SimContext _simContext = new();
	private Scheduler? _scheduler;
	private readonly Dictionary<string, GraphNode> _visualNodes = new();
	private readonly Dictionary<string, EdgeVisualController> _edgeVisuals = new();
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
	/// <returns>The name/ID of the created node</returns>
	public string CreateNode(string nodeType, Vector2 position)
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
				return string.Empty;
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
			return nodeId;
		}

		return string.Empty;
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
			Resizable = false
		};

		// Set tooltip based on node type
		node.TooltipText = logic switch
		{
			EventNode => "📋 Ticket Generator: Creates new work items (1/tick)\nEntry point for your factory pipeline",
			FunctionNode => "⚙️ Code Node: Converts Tickets → Code\nProcesses requirements into executable code",
			SinkNode => "🚀 Deploy: Publishes code to production\nFinal destination - tracks your output metrics",
			GatewayNode => "🔀 Gateway: Splits or merges item flows\nRoutes items to different paths based on conditions",
			BufferNode => "📦 Buffer: Stores items temporarily\nHelps balance load between fast and slow nodes",
			_ => "Node in your factory pipeline"
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
			{
				GD.PrintErr($"  - {error}");
			}

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
			
			// Create edge visual after successful connection
			CreateEdgeVisual(fromId, toId, (int)fromPort, (int)toPort);
		}
	}

	/// <summary>
	/// Handle disconnection request.
	/// </summary>
	private void OnDisconnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
	{
		string fromId = fromNode.ToString();
		string toId = toNode.ToString();

		// Remove edge visual
		var edgeKey = $"{fromId}:{fromPort}->{toId}:{toPort}";
		if (_edgeVisuals.TryGetValue(edgeKey, out var visual))
		{
			visual.QueueFree();
			_edgeVisuals.Remove(edgeKey);
		}

		// Remove edge from simulation
		_simContext.RemoveEdge(fromId, (int)fromPort, toId, (int)toPort);

		// Remove visual connection
		DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);

		GD.Print($"Disconnected {fromId}[{fromPort}] -> {toId}[{toPort}]");
	}

	/// <summary>
	/// Handle delete nodes request.
	/// </summary>
	private void OnDeleteNodesRequest(Godot.Collections.Array<StringName> nodes)
	{
		foreach (var nodeName in nodes)
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

	/// <summary>
	/// Create visual representation of an edge with animated item flow.
	/// </summary>
	private void CreateEdgeVisual(string fromNodeName, string toNodeName, int fromPort, int toPort)
	{
		var edgeKey = $"{fromNodeName}:{fromPort}->{toNodeName}:{toPort}";
		
		if (!_simContext.Edges.ContainsKey(edgeKey))
		{
			GD.PrintErr($"Cannot create visual for non-existent edge: {edgeKey}");
			return;
		}

		var edge = _simContext.Edges[edgeKey];
		
		// Get node positions
		if (!_visualNodes.TryGetValue(fromNodeName, out var fromGraphNode) ||
			!_visualNodes.TryGetValue(toNodeName, out var toGraphNode))
		{
			GD.PrintErr($"Cannot find visual nodes for edge: {edgeKey}");
			return;
		}
		
		var startPos = fromGraphNode.Position + fromGraphNode.Size / 2;
		var endPos = toGraphNode.Position + toGraphNode.Size / 2;

		// Create visual controller
		var visual = new EdgeVisualController
		{
			SimEdge = edge,
			StartPosition = startPos,
			EndPosition = endPos,
			TravelTime = 1.0f
		};

		AddChild(visual);
		_edgeVisuals[edgeKey] = visual;

		GD.Print($"Created edge visual: {edgeKey}");
	}
}
