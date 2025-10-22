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
		{
			return;
		}

		_updateTimer += (float)delta;
		if (_updateTimer < UpdateInterval)
		{
			return;
		}

		_updateTimer = 0.0f;
		UpdateKPIs();
	}

	private void UpdateKPIs()
	{
		if (_kpiLabel == null || GraphEditor == null)
		{
			return;
		}

		var ctx = GraphEditor.GetSimContext();
		var text = "[font_size=18][b]=== FACTORY STATUS ===[/b][/font_size]\n\n";

		// Calculate total items in transit
		int totalItemsInTransit = 0;
		foreach (var edge in ctx.Edges.Values)
		{
			totalItemsInTransit += edge.Queue.Count;
		}

		// Find sink nodes for metrics
		int totalDeploys = 0;
		float avgQuality = 0.0f;
		float throughput = 0.0f;
		int sinkCount = 0;

		foreach (var node in ctx.Nodes.Values)
		{
			if (node is SinkNode sink)
			{
				totalDeploys += sink.TotalConsumed;
				avgQuality += sink.AverageQuality;
				throughput += sink.LastTickConsumed * ctx.TickRate;
				sinkCount++;
			}
		}

		if (sinkCount > 0)
		{
			avgQuality /= sinkCount;
		}

		// Format with emojis and colors
		text += $"[color=green]✓[/color] [b]Total Deploys:[/b] {totalDeploys}\n";
		text += $"[color=cyan]⏱[/color] [b]Throughput:[/b] {throughput:F1}/sec\n";
		text += $"[color=yellow]📦[/color] [b]In Transit:[/b] {totalItemsInTransit}\n";
		
		// Quality indicator with color
		var qualityColor = avgQuality >= 0.8f ? "green" : avgQuality >= 0.5f ? "yellow" : "red";
		text += $"[color={qualityColor}]⭐[/color] [b]Avg Quality:[/b] {avgQuality:F2}\n\n";

		text += "[font_size=14][b]=== INFRASTRUCTURE ===[/b][/font_size]\n";
		text += $"Nodes: {ctx.Nodes.Count} | Edges: {ctx.Edges.Count}\n";
		text += $"Tick: {ctx.TickCount} | Speed: {ctx.SpeedMultiplier}x\n";

		_kpiLabel.Text = text;
	}
}
