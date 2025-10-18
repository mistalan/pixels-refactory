using PixelsRefactory.Simulation.Core;
using System.Collections.Generic;
using System.Linq;

namespace PixelsRefactory.Simulation.Systems;

/// <summary>
/// Provides topological sorting for deterministic node execution order.
/// Ensures nodes are processed in dependency order each tick.
/// </summary>
public static class TopologicalSort
{
    /// <summary>
    /// Performs topological sort on the graph to determine execution order.
    /// Returns node IDs in the order they should be processed.
    /// Returns null if the graph contains cycles.
    /// </summary>
    public static List<string>? Sort(SimContext ctx)
    {
        var nodes = ctx.Nodes.Keys.ToHashSet();
        var inDegree = new Dictionary<string, int>();
        var adjacency = new Dictionary<string, List<string>>();

        // Initialize data structures
        foreach (var nodeId in nodes)
        {
            inDegree[nodeId] = 0;
            adjacency[nodeId] = new List<string>();
        }

        // Build adjacency list and calculate in-degrees
        foreach (var edge in ctx.Edges.Values)
        {
            if (!nodes.Contains(edge.FromNodeId) || !nodes.Contains(edge.ToNodeId))
                continue;

            adjacency[edge.FromNodeId].Add(edge.ToNodeId);
            inDegree[edge.ToNodeId]++;
        }

        // Start with nodes that have no dependencies
        var queue = new Queue<string>();
        foreach (var nodeId in nodes)
        {
            if (inDegree[nodeId] == 0)
                queue.Enqueue(nodeId);
        }

        var result = new List<string>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            // Process neighbors
            foreach (var neighbor in adjacency[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                    queue.Enqueue(neighbor);
            }
        }

        // If we didn't process all nodes, there's a cycle
        if (result.Count != nodes.Count)
            return null;

        return result;
    }

    /// <summary>
    /// Check if the graph is acyclic (DAG).
    /// </summary>
    public static bool IsAcyclic(SimContext ctx)
    {
        return Sort(ctx) != null;
    }
}
