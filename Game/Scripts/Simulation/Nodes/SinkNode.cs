using System.Collections.Generic;
using PixelsRefactory.Simulation.Core;

namespace PixelsRefactory.Simulation.Nodes;

/// <summary>
/// Sink node - consumes items and tracks KPIs.
/// Terminal node in the production pipeline.
/// </summary>
public sealed class SinkNode : INodeLogic
{
	public string Id { get; init; } = string.Empty;
	public string DisplayName { get; init; } = "Sink";

	/// <summary>
	/// Total items consumed.
	/// </summary>
	public int TotalConsumed { get; private set; } = 0;

	/// <summary>
	/// Average quality of consumed items.
	/// </summary>
	public float AverageQuality { get; private set; } = 0.0f;

	/// <summary>
	/// Items consumed per tick (for throughput tracking).
	/// </summary>
	public int LastTickConsumed { get; private set; } = 0;

	private readonly List<float> _qualityHistory = new();

	// Static audio controller reference (set by GraphSceneController)
	private static object? _audioController;

	public static void SetAudioController(object controller)
	{
		_audioController = controller;
	}

	public SinkNode() { }

	public SinkNode(string id)
	{
		Id = id;
		DisplayName = "Sink";
	}

	public void Tick(SimContext ctx)
	{
		// Pull all available items
		var items = ctx.PullFromInputs(Id, maxItems: 100, port: 0);

		LastTickConsumed = items.Count;

		if (items.Count == 0)
		{
			return;
		}

		// Consume items and track metrics
		foreach (var item in items)
		{
			TotalConsumed++;
			_qualityHistory.Add(item.Quality);

			// Play audio feedback using reflection to avoid circular dependency
			if (_audioController != null)
			{
				var method = _audioController.GetType().GetMethod("PlayItemComplete");
				method?.Invoke(_audioController, null);
			}
		}

		// Recalculate average quality
		if (_qualityHistory.Count > 0)
		{
			float sum = 0;
			foreach (var q in _qualityHistory)
			{
				sum += q;
			}

			AverageQuality = sum / _qualityHistory.Count;
		}
	}

	public void Reset()
	{
		TotalConsumed = 0;
		AverageQuality = 0.0f;
		LastTickConsumed = 0;
		_qualityHistory.Clear();
	}
}
