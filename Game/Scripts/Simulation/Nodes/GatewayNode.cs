using PixelsRefactory.Simulation.Core;

namespace PixelsRefactory.Simulation.Nodes;

/// <summary>
/// Gateway node - routes items based on conditions (EPK-style XOR/AND/OR).
/// </summary>
public enum GatewayType
{
    XOR,  // Route to one output based on condition
    AND,  // Duplicate to all outputs
    OR    // Route to first available output
}

public sealed class GatewayNode : INodeLogic
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = "Gateway";
    
    /// <summary>
    /// Type of gateway logic.
    /// </summary>
    public GatewayType Type { get; init; } = GatewayType.XOR;
    
    /// <summary>
    /// For XOR: threshold quality value for routing decision.
    /// Items with quality >= threshold go to port 0, others to port 1.
    /// </summary>
    public float QualityThreshold { get; init; } = 0.5f;

    public GatewayNode() { }

    public GatewayNode(string id, GatewayType type = GatewayType.XOR)
    {
        Id = id;
        DisplayName = $"Gateway ({type})";
        Type = type;
    }

    public void Tick(SimContext ctx)
    {
        var items = ctx.PullFromInputs(Id, maxItems: 100, port: 0);
        
        if (items.Count == 0)
            return;

        switch (Type)
        {
            case GatewayType.XOR:
                // Route based on quality threshold
                foreach (var item in items)
                {
                    int outputPort = item.Quality >= QualityThreshold ? 0 : 1;
                    ctx.PushToOutputs(Id, item, outputPort);
                }
                break;

            case GatewayType.AND:
                // Duplicate to all outputs
                foreach (var item in items)
                {
                    // Push to port 0 and port 1
                    ctx.PushToOutputs(Id, item, port: 0);
                    // Create a copy for port 1
                    var itemCopy = new Item(item.Type, item.Quality, item.Size);
                    ctx.PushToOutputs(Id, itemCopy, port: 1);
                }
                break;

            case GatewayType.OR:
                // Route to first available output (simplified: just use port 0)
                foreach (var item in items)
                {
                    ctx.PushToOutputs(Id, item, port: 0);
                }
                break;
        }
    }
}
