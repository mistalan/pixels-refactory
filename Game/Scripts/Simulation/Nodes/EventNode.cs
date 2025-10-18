using PixelsRefactory.Simulation.Core;

namespace PixelsRefactory.Simulation.Nodes;

/// <summary>
/// Event node - generates items at a specified rate.
/// Entry point for items into the factory.
/// </summary>
public sealed class EventNode : INodeLogic
{
	public string Id { get; init; } = string.Empty;
	public string DisplayName { get; init; } = "Event";

	/// <summary>
	/// Type of item this event generates.
	/// </summary>
	public ItemType ItemType { get; init; } = ItemType.Ticket;

	/// <summary>
	/// How many items to generate per tick (0 = none, 1+ = that many).
	/// </summary>
	public int GenerationRate { get; init; } = 1;

	/// <summary>
	/// Initial quality of generated items.
	/// </summary>
	public float Quality { get; init; } = 1.0f;

	/// <summary>
	/// Size of generated items.
	/// </summary>
	public int Size { get; init; } = 1;

	private int _tickCounter = 0;

	public EventNode() { }

	public EventNode(string id, ItemType itemType = ItemType.Ticket, int generationRate = 1)
	{
		Id = id;
		DisplayName = $"Event ({itemType})";
		ItemType = itemType;
		GenerationRate = generationRate;
	}

	public void Tick(SimContext ctx)
	{
		_tickCounter++;

		// Generate items at the specified rate
		for (int i = 0; i < GenerationRate; i++)
		{
			var item = new Item(ItemType, Quality, Size);
			ctx.PushToOutputs(Id, item, port: 0);
		}
	}
}
