using Microsoft.Extensions.Configuration;

namespace Simple.Logging;

public class LoggingOptions
{
    public string AppName { get; set; }
    public string AppInsightKey { get; set; }

    public LoggingOptions()
    {

    }

    public LoggingOptions(IConfiguration configuration)
    {
        AppInsightKey = configuration["AppInsights:IKey"];
    }
}