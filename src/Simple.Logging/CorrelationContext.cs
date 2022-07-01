namespace Simple.Logging;

public class CorrelationContext : ICorrelationContext
{
    private readonly AsyncLocal<string> correlationId = new AsyncLocal<string>();

    public string GetCorrelationId()
    {
        return string.IsNullOrWhiteSpace(correlationId.Value) ? New() : correlationId.Value;
    }

    public string New()
    {
        correlationId.Value = Guid.NewGuid().ToString();
        return correlationId.Value;
    }

    public void SetCorrelationId(string id, bool shouldOverride = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Correlation id cannot be null or empty", nameof(id));
        }

        if (!shouldOverride && !string.IsNullOrWhiteSpace(correlationId.Value))
        {
            throw new InvalidOperationException(
                "Correlation Id can not be overridden as it is already set. Set shouldOverride parameter to true");
        }

        correlationId.Value = id;
    }
}