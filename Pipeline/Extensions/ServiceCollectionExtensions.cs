using Microsoft.Extensions.DependencyInjection;

namespace Pipeline.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="PipelineRunner{TContext}"/> and the
    /// built-in timing + progress middlewares.
    /// </summary>
    public static IServiceCollection AddPipeline<TContext>(this IServiceCollection services)
        where TContext : class
    {
        services.AddSingleton<IOutputWriter, ConsoleOutputWriter>();
        services.AddSingleton<IPipeline<TContext>, PipelineRunner<TContext>>();
        return services;
    }

    /// <summary>
    /// Convenience: build a runner, add middleware, and register stages in one call.
    /// </summary>
    public static PipelineRunner<TContext> BuildPipeline<TContext>(
        this IServiceCollection services,
        Action<PipelineRunner<TContext>>? configure = null)
        where TContext : class
    {
        var runner = new PipelineRunner<TContext>(
            services.BuildServiceProvider().GetRequiredService<ILogger<PipelineRunner<TContext>>>());

        runner.UseTiming(new StageTimingMiddleware(
            services.BuildServiceProvider().GetRequiredService<ILogger<StageTimingMiddleware>>()));
        runner.UseMiddleware((ctx, next, ct) => new ProgressMiddleware(new ConsoleOutputWriter()).Invoke(ctx, next, ct));

        configure?.Invoke(runner);
        return runner;
    }
}
