using PixelsRefactory.Simulation.Core;
using System;

namespace PixelsRefactory.Simulation.Nodes;

/// <summary>
/// Function node - processes items with throughput and latency.
/// Can transform item types and affect quality.
/// </summary>
public sealed class FunctionNode : INodeLogic
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = "Function";
    
    /// <summary>
    /// Maximum number of items to process per tick.
    /// </summary>
    public int Throughput { get; init; } = 1;
    
    /// <summary>
    /// Number of ticks items wait inside this node before processing.
    /// 0 = instant processing.
    /// </summary>
    public int Latency { get; init; } = 0;
    
    /// <summary>
    /// Optional: transform input items to this type.
    /// If null, items pass through unchanged.
    /// </summary>
    public ItemType? OutputType { get; init; } = null;
    
    /// <summary>
    /// Quality multiplier applied to processed items (1.0 = no change).
    /// </summary>
    public float QualityMultiplier { get; init; } = 1.0f;

    private int _currentLatencyTicks = 0;

    public FunctionNode() { }

    public FunctionNode(string id, int throughput = 1, int latency = 0, ItemType? outputType = null)
    {
        Id = id;
        DisplayName = $"Function";
        Throughput = throughput;
        Latency = latency;
        OutputType = outputType;
    }

    public void Tick(SimContext ctx)
    {
        // Pull items from inputs
        var items = ctx.PullFromInputs(Id, Throughput, port: 0);
        
        if (items.Count == 0)
        {
            _currentLatencyTicks = 0;
            return;
        }

        // Apply latency
        if (Latency > 0)
        {
            _currentLatencyTicks++;
            if (_currentLatencyTicks < Latency)
            {
                // Items are still being processed, put them back (simplified)
                // In a real implementation, we'd queue them internally
                return;
            }
            _currentLatencyTicks = 0;
        }

        // Process items
        foreach (var item in items)
        {
            // Transform type if specified
            if (OutputType.HasValue)
            {
                item.Type = OutputType.Value;
            }
            
            // Apply quality modification
            item.Quality *= QualityMultiplier;
            item.Quality = Math.Clamp(item.Quality, 0.0f, 2.0f);
            
            // Push to outputs
            ctx.PushToOutputs(Id, item, port: 0);
        }
    }
}
