namespace Pipeline.Middleware;

public sealed class ProgressMiddleware(IOutputWriter writer)
{
    public async Task Invoke<TContext>(
        StageExecutionContext<TContext> context,
        Func<Task> next,
        CancellationToken cancellationToken)
        where TContext : class
    {
        var label = $"[{context.StageIndex + 1}/{context.StageTotal}] {context.Stage.Name}";
        writer.WriteLine($"{label} … ");
        var sw = Stopwatch.StartNew();

        try
        {
            await next().ConfigureAwait(false);
            writer.WriteLine($"✓ {sw.Elapsed:hh\\:mm\\:ss}");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            writer.WriteLine($"✗ cancelled after {sw.Elapsed:hh\\:mm\\:ss}");
            throw;
        }
        catch (Exception ex)
        {
            writer.WriteLine($"✗ {ex.GetType().Name} after {sw.Elapsed:hh\\:mm\\:ss}");
            throw;
        }

        writer.WriteLine();
    }
}
