using Godot;
using PixelsRefactory.UI;

namespace PixelsRefactory.GraphEditor;

/// <summary>
/// Main scene controller that wires together the graph editor, simulation, and UI.
/// </summary>
public partial class GraphSceneController : Control
{
    private GraphEditorController? _graphEditor;
    private SimulationController? _simulationController;
    private KPIPanel? _kpiPanel;
    private Vector2 _nextNodePosition = new Vector2(200, 200);

    public override void _Ready()
    {
        // Get references to child nodes
        _graphEditor = GetNodeOrNull<GraphEditorController>("GraphEditor");
        _simulationController = GetNodeOrNull<SimulationController>("ControlPanel/SimulationController");
        _kpiPanel = GetNodeOrNull<KPIPanel>("KPIPanel");

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
            createEventBtn.Pressed += () => OnCreateNodePressed("event");
        if (createFunctionBtn != null)
            createFunctionBtn.Pressed += () => OnCreateNodePressed("function");
        if (createGatewayBtn != null)
            createGatewayBtn.Pressed += () => OnCreateNodePressed("gateway");
        if (createBufferBtn != null)
            createBufferBtn.Pressed += () => OnCreateNodePressed("buffer");
        if (createSinkBtn != null)
            createSinkBtn.Pressed += () => OnCreateNodePressed("sink");

        GD.Print("GraphSceneController ready");
        GD.Print("Use toolbar buttons to create nodes, then drag to connect them");
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
