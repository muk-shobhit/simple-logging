using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Simple.Logging;

public class CorrelationIdMiddleware
{
    private readonly ICorrelationContext correlationContext;
    private readonly RequestDelegate next;
    private readonly ILogger<CorrelationIdMiddleware> logger;

    public CorrelationIdMiddleware(RequestDelegate next,
        ICorrelationContext correlationContext, ILogger<CorrelationIdMiddleware> logger)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.correlationContext = correlationContext;
        this.logger = logger;
    }

    public Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId) &&
            !string.IsNullOrEmpty(correlationId))
        {
            context.TraceIdentifier = correlationId;
        }

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add("X-Correlation-ID", new[] { context.TraceIdentifier });
            return Task.CompletedTask;
        });

        correlationContext.SetCorrelationId(context.TraceIdentifier, true);
        var logEntry = new Dictionary<string, object>
        {
            {"correlationId", correlationContext.GetCorrelationId()},
        };
        using (logger.BeginScope(logEntry))
        {
            return next(context);
        }
    }
}