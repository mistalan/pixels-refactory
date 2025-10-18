using PixelsRefactory.Simulation.Core;
using System.Collections.Generic;

namespace PixelsRefactory.Simulation.Nodes;

/// <summary>
/// Buffer node - stores items temporarily, useful for load balancing.
/// </summary>
public sealed class BufferNode : INodeLogic
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = "Buffer";
    
    /// <summary>
    /// Maximum capacity of the buffer.
    /// </summary>
    public int Capacity { get; init; } = 10;
    
    /// <summary>
    /// How many items to release per tick.
    /// </summary>
    public int ReleaseRate { get; init; } = 1;

    private readonly Queue<Item> _internalQueue = new();

    public BufferNode() { }

    public BufferNode(string id, int capacity = 10, int releaseRate = 1)
    {
        Id = id;
        DisplayName = "Buffer";
        Capacity = capacity;
        ReleaseRate = releaseRate;
    }

    public void Tick(SimContext ctx)
    {
        // Pull items from inputs and store in internal queue
        int availableSpace = Capacity - _internalQueue.Count;
        if (availableSpace > 0)
        {
            var items = ctx.PullFromInputs(Id, availableSpace, port: 0);
            foreach (var item in items)
            {
                _internalQueue.Enqueue(item);
            }
        }

        // Release items at the specified rate
        int releaseCount = 0;
        while (releaseCount < ReleaseRate && _internalQueue.Count > 0)
        {
            var item = _internalQueue.Dequeue();
            ctx.PushToOutputs(Id, item, port: 0);
            releaseCount++;
        }
    }

    public int GetCurrentLoad()
    {
        return _internalQueue.Count;
    }
}
