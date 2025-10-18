using PixelsRefactory.Simulation.Core;
using System.Collections.Generic;

namespace PixelsRefactory.Simulation.Systems;

/// <summary>
/// Manages the tick-based simulation scheduler.
/// Executes nodes in topological order each tick.
/// </summary>
public sealed class Scheduler
{
	private List<string>? _executionOrder;
	private readonly SimContext _context;

	public Scheduler(SimContext context)
	{
		_context = context;
	}

	/// <summary>
	/// Recalculate the execution order based on current graph topology.
	/// Must be called when graph structure changes.
	/// </summary>
	public bool RecalculateOrder()
	{
		_executionOrder = TopologicalSort.Sort(_context);
		return _executionOrder != null;
	}

	/// <summary>
	/// Execute one simulation tick.
	/// Processes all nodes in topological order.
	/// </summary>
	public void Tick()
	{
		if (_executionOrder == null)
		{
			if (!RecalculateOrder())
				return; // Graph has cycles, cannot execute
		}

		// Execute each node in order
		foreach (var nodeId in _executionOrder)
		{
			if (_context.Nodes.TryGetValue(nodeId, out var node))
			{
				node.Tick(_context);
			}
		}

		_context.TickCount++;
	}

	/// <summary>
	/// Get the current execution order.
	/// </summary>
	public IReadOnlyList<string>? GetExecutionOrder()
	{
		return _executionOrder?.AsReadOnly();
	}
}
