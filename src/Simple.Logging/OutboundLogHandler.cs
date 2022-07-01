using Microsoft.Extensions.Logging;

namespace Simple.Logging;

public class OutboundLogHandler : DelegatingHandler
{
    private readonly ICorrelationContext correlationContext;
    private readonly ILogger<OutboundLogHandler> logger;

    public OutboundLogHandler(ILogger<OutboundLogHandler> logger,
        ICorrelationContext correlationContext)
    {
        this.logger = logger;
        this.correlationContext = correlationContext;
    }

    private static string GetLogMessage(HttpRequestMessage request, HttpResponseMessage response)
    {
        return
            $"HTTP {request?.Method} {request?.RequestUri?.PathAndQuery} " +
            $"responded with {response?.StatusCode}";
    }

    private static Dictionary<string, object> GetRequestProperties(HttpRequestMessage request) => new()
    {
        { "method", request.Method.Method },
        { "path", request.RequestUri?.AbsolutePath },
        { "queryString", request.RequestUri?.Query }
    };

    private bool LogFailedResponseException(HttpRequestMessage request, Exception exception,
        Dictionary<string, object> logEntry)
    {
        var msg = $"HTTP {request.Method} {request.RequestUri?.PathAndQuery}";

        logEntry["request"] = GetRequestProperties(request);
        logEntry["isSuccess"] = false;

        using (logger.BeginScope(logEntry))
        {
            logger.LogError(exception, msg);
        }

        return false;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        HttpResponseMessage response = null;

        var logEntry = new Dictionary<string, object>
        {
            {"correlationId", correlationContext.GetCorrelationId()},
            {"type", "Outbound"},
            {"request", GetRequestProperties(request)}
        };

        try
        {
            response = await base.SendAsync(request, cancellationToken);

            logEntry.Add("response", new Dictionary<string, object> { { "statusCode", (int)response.StatusCode } });
            logEntry.Add("isSuccess", response.IsSuccessStatusCode);

            using (logger.BeginScope(logEntry))
            {
                logger.LogInformation(GetLogMessage(request, response));
            }
        }
        catch (Exception ex) when (
            LogFailedResponseException(request, ex, logEntry))
        {
        }

        return response;
    }
}