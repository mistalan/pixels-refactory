using Godot;
using PixelsRefactory.UI;
using PixelsRefactory.Audio;
using PixelsRefactory.Simulation.Nodes;

namespace PixelsRefactory.GraphEditor;

/// <summary>
/// Main scene controller that wires together the graph editor, simulation, and UI.
/// </summary>
public partial class GraphSceneController : Control
{
	private GraphEditorController? _graphEditor;
	private SimulationController? _simulationController;
	private KPIPanel? _kpiPanel;
	private AudioFeedbackController? _audioController;
	private Vector2 _nextNodePosition = new Vector2(200, 200);

	public override void _Ready()
	{
		// Get references to child nodes
		_graphEditor = GetNodeOrNull<GraphEditorController>("GraphEditor");
		_simulationController = GetNodeOrNull<SimulationController>("ControlPanel/SimulationController");
		_kpiPanel = GetNodeOrNull<KPIPanel>("KPIPanel");

		// Initialize audio
		_audioController = new AudioFeedbackController();
		AddChild(_audioController);
		SinkNode.SetAudioController(_audioController);

		// Wire up references
		if (_simulationController != null && _graphEditor != null)
		{
			_simulationController.GraphEditor = _graphEditor;
		}
		if (_kpiPanel != null && _graphEditor != null)
		{
			_kpiPanel.GraphEditor = _graphEditor;
		}

		// Connect toolbar button signals
		var createEventBtn = GetNodeOrNull<Button>("Toolbar/ToolbarButtons/CreateEventButton");
		var createFunctionBtn = GetNodeOrNull<Button>("Toolbar/ToolbarButtons/CreateFunctionButton");
		var createGatewayBtn = GetNodeOrNull<Button>("Toolbar/ToolbarButtons/CreateGatewayButton");
		var createBufferBtn = GetNodeOrNull<Button>("Toolbar/ToolbarButtons/CreateBufferButton");
		var createSinkBtn = GetNodeOrNull<Button>("Toolbar/ToolbarButtons/CreateSinkButton");

		if (createEventBtn != null)
		{
			createEventBtn.Pressed += () => OnCreateNodePressed("event");
		}

		if (createFunctionBtn != null)
		{
			createFunctionBtn.Pressed += () => OnCreateNodePressed("function");
		}

		if (createGatewayBtn != null)
		{
			createGatewayBtn.Pressed += () => OnCreateNodePressed("gateway");
		}

		if (createBufferBtn != null)
		{
			createBufferBtn.Pressed += () => OnCreateNodePressed("buffer");
		}

		if (createSinkBtn != null)
		{
			createSinkBtn.Pressed += () => OnCreateNodePressed("sink");
		}

		GD.Print("GraphSceneController ready");
		GD.Print("Use toolbar buttons to create nodes, then drag to connect them");

		// Load starter factory for first-time player experience
		LoadStarterFactory();
	}

	private void LoadStarterFactory()
	{
		if (_graphEditor == null)
		{
			GD.PrintErr("Cannot load starter factory: GraphEditor is null");
			return;
		}

		GD.Print("Loading starter factory for first-time player experience...");

		// Create EventNode (Ticket Generator) at position (100, 150)
		var eventNodeName = _graphEditor.CreateNode("event", new Vector2(100, 150));
		
		// Create FunctionNode (Code Processor) at position (400, 150)
		var functionNodeName = _graphEditor.CreateNode("function", new Vector2(400, 150));
		
		// Create SinkNode (Deploy) at position (700, 150)
		var sinkNodeName = _graphEditor.CreateNode("sink", new Vector2(700, 150));

		// Connect the nodes using CallDeferred to ensure nodes are fully initialized
		_graphEditor.CallDeferred("connect_node", eventNodeName, 0, functionNodeName, 0);
		_graphEditor.CallDeferred("connect_node", functionNodeName, 0, sinkNodeName, 0);

		GD.Print($"Starter factory loaded: {eventNodeName} → {functionNodeName} → {sinkNodeName}");

		// Update next position to avoid overlap with starter nodes
		_nextNodePosition = new Vector2(1000, 150);
	}

	private void OnCreateNodePressed(string nodeType)
	{
		if (_graphEditor == null)
		{
			GD.PrintErr("GraphEditor not found");
			return;
		}

		// Create node at the next position
		_graphEditor.CreateNode(nodeType, _nextNodePosition);

		// Update position for next node
		_nextNodePosition.X += 250;
		if (_nextNodePosition.X > 1500)
		{
			_nextNodePosition.X = 200;
			_nextNodePosition.Y += 150;
		}
	}
}
