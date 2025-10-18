namespace PixelsRefactory.Simulation.Core;

/// <summary>
/// Interface for all node types in the simulation.
/// Each node implements tick-based processing logic.
/// </summary>
public interface INodeLogic
{
    /// <summary>
    /// Unique identifier for this node instance.
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Human-readable display name.
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// Process one simulation tick.
    /// Pulls items from input queues and pushes to output queues.
    /// </summary>
    /// <param name="ctx">The simulation context</param>
    void Tick(SimContext ctx);
}
