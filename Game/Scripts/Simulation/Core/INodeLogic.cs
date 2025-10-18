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
	public string Id { get; }

	/// <summary>
	/// Human-readable display name.
	/// </summary>
	public string DisplayName { get; }

	/// <summary>
	/// Process one simulation tick.
	/// Pulls items from input queues and pushes to output queues.
	/// </summary>
	/// <param name="ctx">The simulation context</param>
	public void Tick(SimContext ctx);
}
