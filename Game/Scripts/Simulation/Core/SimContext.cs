using System.Collections.Generic;

namespace PixelsRefactory.Simulation.Core;

/// <summary>
/// Central simulation context that holds the world state.
/// Owns all nodes, edges, and provides access to queues.
/// </summary>
public sealed class SimContext
{
    /// <summary>
    /// All nodes in the simulation, keyed by ID.
    /// </summary>
    public Dictionary<string, INodeLogic> Nodes { get; } = new();
    
    /// <summary>
    /// All edges in the simulation, keyed by edge key.
    /// </summary>
    public Dictionary<string, Edge> Edges { get; } = new();
    
    /// <summary>
    /// Tick rate (ticks per second).
    /// </summary>
    public float TickRate { get; set; } = 10.0f;
    
    /// <summary>
    /// Current simulation tick counter.
    /// </summary>
    public long TickCount { get; set; } = 0;
    
    /// <summary>
    /// Whether the simulation is currently running.
    /// </summary>
    public bool IsRunning { get; set; } = false;
    
    /// <summary>
    /// Simulation speed multiplier (1.0 = normal, 2.0 = double speed).
    /// </summary>
    public float SpeedMultiplier { get; set; } = 1.0f;

    /// <summary>
    /// Add a node to the simulation.
    /// </summary>
    public void AddNode(INodeLogic node)
    {
        Nodes[node.Id] = node;
    }

    /// <summary>
    /// Remove a node from the simulation.
    /// </summary>
    public bool RemoveNode(string nodeId)
    {
        return Nodes.Remove(nodeId);
    }

    /// <summary>
    /// Add an edge to the simulation.
    /// </summary>
    public void AddEdge(Edge edge)
    {
        Edges[edge.GetKey()] = edge;
    }

    /// <summary>
    /// Remove an edge from the simulation.
    /// </summary>
    public bool RemoveEdge(string fromNodeId, int fromPort, string toNodeId, int toPort)
    {
        var edge = new Edge(fromNodeId, toNodeId, fromPort, toPort);
        return Edges.Remove(edge.GetKey());
    }

    /// <summary>
    /// Get the input queue for a specific node and port.
    /// Returns all items in edges pointing to this node/port.
    /// </summary>
    public List<Item> GetInputItems(string nodeId, int port = 0)
    {
        var items = new List<Item>();
        foreach (var edge in Edges.Values)
        {
            if (edge.ToNodeId == nodeId && edge.ToPort == port)
            {
                items.AddRange(edge.Queue);
            }
        }
        return items;
    }

    /// <summary>
    /// Push an item to all output edges from a specific node and port.
    /// </summary>
    public void PushToOutputs(string nodeId, Item item, int port = 0)
    {
        foreach (var edge in Edges.Values)
        {
            if (edge.FromNodeId == nodeId && edge.FromPort == port)
            {
                edge.Queue.Enqueue(item);
            }
        }
    }

    /// <summary>
    /// Pull and remove items from input edges for a specific node and port.
    /// </summary>
    public List<Item> PullFromInputs(string nodeId, int maxItems, int port = 0)
    {
        var items = new List<Item>();
        foreach (var edge in Edges.Values)
        {
            if (edge.ToNodeId == nodeId && edge.ToPort == port)
            {
                while (items.Count < maxItems && edge.Queue.Count > 0)
                {
                    items.Add(edge.Queue.Dequeue());
                }
                if (items.Count >= maxItems)
                    break;
            }
        }
        return items;
    }

    /// <summary>
    /// Clear all simulation state.
    /// </summary>
    public void Clear()
    {
        Nodes.Clear();
        Edges.Clear();
        TickCount = 0;
        IsRunning = false;
    }
}
