using System.Collections.Generic;

namespace PixelsRefactory.Simulation.Core;

/// <summary>
/// Represents a directed edge in the graph connecting two nodes.
/// </summary>
public sealed class Edge
{
    /// <summary>
    /// Source node ID.
    /// </summary>
    public string FromNodeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Destination node ID.
    /// </summary>
    public string ToNodeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional: which output port from the source node.
    /// </summary>
    public int FromPort { get; set; } = 0;
    
    /// <summary>
    /// Optional: which input port on the destination node.
    /// </summary>
    public int ToPort { get; set; } = 0;

    /// <summary>
    /// FIFO queue of items traveling along this edge.
    /// </summary>
    public Queue<Item> Queue { get; set; } = new();

    public Edge() { }

    public Edge(string fromNodeId, string toNodeId, int fromPort = 0, int toPort = 0)
    {
        FromNodeId = fromNodeId;
        ToNodeId = toNodeId;
        FromPort = fromPort;
        ToPort = toPort;
    }

    public string GetKey()
    {
        return $"{FromNodeId}:{FromPort}->{ToNodeId}:{ToPort}";
    }

    public override string ToString()
    {
        return $"{FromNodeId}[{FromPort}] -> {ToNodeId}[{ToPort}] (Items: {Queue.Count})";
    }
}
