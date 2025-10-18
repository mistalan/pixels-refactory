using Godot;
using PixelsRefactory.GraphEditor;
using PixelsRefactory.Simulation.Nodes;

namespace PixelsRefactory.UI;

/// <summary>
/// Displays KPIs and metrics from sink nodes.
/// </summary>
public partial class KPIPanel : Control
{
	[Export] public GraphEditorController? GraphEditor { get; set; }
	
	private Label? _kpiLabel;
	private float _updateTimer = 0.0f;
	private const float UpdateInterval = 0.5f; // Update every 0.5 seconds

	public override void _Ready()
	{
		_kpiLabel = GetNodeOrNull<Label>("KPILabel");
		GD.Print("KPIPanel ready");
	}

	public override void _Process(double delta)
	{
		if (GraphEditor == null || _kpiLabel == null)
			return;

		_updateTimer += (float)delta;
		if (_updateTimer < UpdateInterval)
			return;

		_updateTimer = 0.0f;
		UpdateKPIs();
	}

	private void UpdateKPIs()
	{
		if (_kpiLabel == null || GraphEditor == null)
			return;

		var ctx = GraphEditor.GetSimContext();
		var text = "=== KPIs ===\n";
		
		int totalNodes = ctx.Nodes.Count;
		int totalEdges = ctx.Edges.Count;
		int totalItemsInTransit = 0;

		foreach (var edge in ctx.Edges.Values)
		{
			totalItemsInTransit += edge.Queue.Count;
		}

		text += $"Nodes: {totalNodes}\n";
		text += $"Edges: {totalEdges}\n";
		text += $"Items in transit: {totalItemsInTransit}\n";
		text += "\n";

		// Find sink nodes and display their metrics
		foreach (var node in ctx.Nodes.Values)
		{
			if (node is SinkNode sink)
			{
				text += $"Sink '{sink.DisplayName}':\n";
				text += $"  Total: {sink.TotalConsumed}\n";
				text += $"  Avg Quality: {sink.AverageQuality:F2}\n";
				text += $"  Last Tick: {sink.LastTickConsumed}\n";
			}
		}

		_kpiLabel.Text = text;
	}
}
