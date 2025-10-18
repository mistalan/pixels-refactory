namespace PixelsRefactory.Simulation.Core;

/// <summary>
/// Types of items that flow through the production pipeline.
/// </summary>
public enum ItemType
{
	Ticket,   // Initial requirements or user stories
	Spec,     // Specifications or designs
	Code,     // Source code
	Test,     // Test results
	Build,    // Built artifacts
	Doc       // Documentation
}

/// <summary>
/// Represents an item flowing through the factory.
/// Items move through edges between nodes in the graph.
/// </summary>
public sealed class Item
{
	/// <summary>
	/// The type of this item.
	/// </summary>
	public ItemType Type { get; set; }
	
	/// <summary>
	/// Quality metric (0.0 to 1.0+). Can be degraded or improved by nodes.
	/// </summary>
	public float Quality { get; set; } = 1.0f;
	
	/// <summary>
	/// Optional size/complexity metric.
	/// </summary>
	public int Size { get; set; } = 1;

	public Item(ItemType type, float quality = 1.0f, int size = 1)
	{
		Type = type;
		Quality = quality;
		Size = size;
	}

	public override string ToString()
	{
		return $"{Type}(Q:{Quality:F2}, S:{Size})";
	}
}
