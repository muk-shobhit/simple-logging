using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Simple.Logging;

public class LoggingStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            next(app);
        };
    }
}