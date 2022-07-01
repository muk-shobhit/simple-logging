namespace Simple.Logging;

public class CorrelationIdHandler : DelegatingHandler
{
    private readonly ICorrelationContext correlationContext;

    public CorrelationIdHandler(ICorrelationContext correlationContext)
    {
        this.correlationContext = correlationContext;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!request.Headers.TryGetValues("X-Correlation-ID", out _) &&
            !string.IsNullOrWhiteSpace(correlationContext.GetCorrelationId()))
        {
            request.Headers.Add("X-Correlation-ID", correlationContext.GetCorrelationId());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}