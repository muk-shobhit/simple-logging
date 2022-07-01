namespace Simple.Logging;

public interface ICorrelationContext
{
    string GetCorrelationId();
    string New();
    void SetCorrelationId(string correlationId, bool shouldOverride = false);
}