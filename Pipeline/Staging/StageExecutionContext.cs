namespace Pipeline.Staging;

public sealed record StageExecutionContext<TContext>(
    IPipelineStage<TContext> Stage,
    TContext PipelineContext,
    int StageIndex,
    int StageTotal) where TContext : class
{
    /// <summary>Accumulated metrics populated by middleware (e.g. the timing middleware).</summary>
    public StageMetric? Metric { get; set; }
}
