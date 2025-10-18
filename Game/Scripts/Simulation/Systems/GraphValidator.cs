using PixelsRefactory.Simulation.Core;
using System.Collections.Generic;

namespace PixelsRefactory.Simulation.Systems;

/// <summary>
/// Validates graph structure and constraints.
/// </summary>
public static class GraphValidator
{
	public sealed class ValidationResult
	{
		public bool IsValid { get; set; }
		public List<string> Errors { get; set; } = new();
		public List<string> Warnings { get; set; } = new();
	}

	/// <summary>
	/// Validate the entire graph structure.
	/// </summary>
	public static ValidationResult Validate(SimContext ctx)
	{
		var result = new ValidationResult { IsValid = true };

		// Check for cycles
		if (!TopologicalSort.IsAcyclic(ctx))
		{
			result.IsValid = false;
			result.Errors.Add("Graph contains cycles. EPK graphs must be acyclic (DAG).");
		}

		// Check for disconnected edges
		foreach (var edge in ctx.Edges.Values)
		{
			if (!ctx.Nodes.ContainsKey(edge.FromNodeId))
			{
				result.IsValid = false;
				result.Errors.Add($"Edge references non-existent source node: {edge.FromNodeId}");
			}
			if (!ctx.Nodes.ContainsKey(edge.ToNodeId))
			{
				result.IsValid = false;
				result.Errors.Add($"Edge references non-existent target node: {edge.ToNodeId}");
			}
		}

		// Check for nodes without outputs (except sinks)
		foreach (var node in ctx.Nodes.Values)
		{
			bool hasOutput = false;
			foreach (var edge in ctx.Edges.Values)
			{
				if (edge.FromNodeId == node.Id)
				{
					hasOutput = true;
					break;
				}
			}

			if (!hasOutput && !node.DisplayName.Contains("Sink"))
			{
				result.Warnings.Add($"Node '{node.DisplayName}' ({node.Id}) has no outputs.");
			}
		}

		return result;
	}
}
