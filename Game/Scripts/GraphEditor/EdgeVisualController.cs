using Godot;
using PixelsRefactory.Simulation.Core;
using System.Collections.Generic;

namespace PixelsRefactory.GraphEditor;

/// <summary>
/// Visualizes items flowing along an edge between two nodes.
/// Creates animated dots that move from source to destination.
/// </summary>
public partial class EdgeVisualController : Node2D
{
	/// <summary>
	/// Reference to the simulation edge data.
	/// </summary>
	public Edge? SimEdge { get; set; }

	/// <summary>
	/// Start position (source node center).
	/// </summary>
	public Vector2 StartPosition { get; set; }

	/// <summary>
	/// End position (destination node center).
	/// </summary>
	public Vector2 EndPosition { get; set; }

	/// <summary>
	/// Travel time for items to move from start to end (in seconds).
	/// </summary>
	[Export]
	public float TravelTime { get; set; } = 1.0f;

	/// <summary>
	/// Visual representations of items currently traveling.
	/// </summary>
	private readonly List<VisualItem> _visualItems = new();

	/// <summary>
	/// Pool of ColorRect nodes for rendering items.
	/// </summary>
	private readonly List<ColorRect> _itemPool = new();

	private class VisualItem
	{
		public ColorRect Node { get; set; } = null!;
		public float Progress { get; set; } = 0.0f; // 0.0 to 1.0
		public ItemType Type { get; set; }
	}

	public override void _Ready()
	{
		// Pre-create a pool of 20 ColorRect nodes for performance
		for (int i = 0; i < 20; i++)
		{
			var rect = new ColorRect
			{
				Size = new Vector2(8, 8),
				Visible = false
			};
			AddChild(rect);
			_itemPool.Add(rect);
		}
	}

	public override void _Process(double delta)
	{
		if (SimEdge == null) return;

		// Sync visual items with simulation queue
		SyncWithSimulation();

		// Update positions of traveling items
		foreach (var visualItem in _visualItems)
		{
			visualItem.Progress += (float)delta / TravelTime;

			if (visualItem.Progress >= 1.0f)
			{
				visualItem.Progress = 1.0f;
			}

			// Lerp position from start to end
			var position = StartPosition.Lerp(EndPosition, visualItem.Progress);
			visualItem.Node.Position = position - visualItem.Node.Size / 2;
		}

		// Remove completed items (they've reached the destination)
		_visualItems.RemoveAll(vi => vi.Progress >= 1.0f);
	}

	private void SyncWithSimulation()
	{
		if (SimEdge == null) return;

		int queueCount = SimEdge.Queue.Count;
		int currentVisuals = _visualItems.Count;

		// Add new visual items if queue has more items than we're showing
		if (queueCount > currentVisuals)
		{
			int toAdd = Mathf.Min(queueCount - currentVisuals, 5); // Add max 5 per frame
			for (int i = 0; i < toAdd; i++)
			{
				AddVisualItem();
			}
		}

		// Remove visual items if queue has fewer items
		while (_visualItems.Count > queueCount && _visualItems.Count > 0)
		{
			RemoveVisualItem();
		}
	}

	private void AddVisualItem()
	{
		if (SimEdge == null) return;

		// Get an item from the pool
		var colorRect = GetAvailableRect();
		if (colorRect == null) return;

		// Peek at the queue to determine item type
		var items = new List<Item>(SimEdge.Queue);
		var itemType = items.Count > 0 ? items[0].Type : ItemType.Ticket;

		// Set color based on item type
		colorRect.Color = GetColorForItemType(itemType);
		colorRect.Visible = true;

		_visualItems.Add(new VisualItem
		{
			Node = colorRect,
			Progress = 0.0f,
			Type = itemType
		});
	}

	private void RemoveVisualItem()
	{
		if (_visualItems.Count == 0) return;

		var visualItem = _visualItems[0];
		visualItem.Node.Visible = false;
		_visualItems.RemoveAt(0);
	}

	private ColorRect? GetAvailableRect()
	{
		foreach (var rect in _itemPool)
		{
			if (!rect.Visible)
			{
				return rect;
			}
		}
		return null; // Pool exhausted
	}

	private static Color GetColorForItemType(ItemType type)
	{
		return type switch
		{
			ItemType.Ticket => new Color(0, 1, 1),      // Cyan
			ItemType.Spec => new Color(0.5f, 0.5f, 1),  // Light blue
			ItemType.Code => new Color(0, 1, 0),        // Green
			ItemType.Test => new Color(1, 1, 0),        // Yellow
			ItemType.Build => new Color(1, 0.5f, 0),    // Orange
			ItemType.Doc => new Color(0.8f, 0.8f, 0.8f),// Gray
			_ => new Color(1, 1, 1)                     // White
		};
	}
}
