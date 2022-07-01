using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Simple.Logging;

public static class ServiceCollectionExtension
{
    public static void AddSimpleLogging(this IServiceCollection services, LoggingOptions loggingOptions = null)
    {
        loggingOptions ??= new LoggingOptions(GetConfiguration());

        if (string.IsNullOrEmpty(loggingOptions.AppInsightKey))
            loggingOptions.AppInsightKey = GetConfiguration()["AppInsights:IKey"];

        services.TryAddSingleton<ICorrelationContext, CorrelationContext>();
        services.AddSingleton<IStartupFilter, LoggingStartupFilter>();

        var logger = new LoggerConfiguration()
            .ConfigureBaseLogging(loggingOptions.AppName)
            .AddApplicationInsights(loggingOptions)
            .CreateLogger();

        services.AddLogging(lb => lb.AddSerilog(logger));
    }

    public static IHttpClientBuilder AddSimpleHttpClient<TClient, TImplementation>(this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        services ??= new ServiceCollection();

        return services
            .AddHttpClient<TClient, TImplementation>()
            .AddLogHandlers();
    }

    public static IHttpClientBuilder AddLogHandlers(this IHttpClientBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.TryAddSingleton<ICorrelationContext, CorrelationContext>();
        builder.Services.TryAddTransient<OutboundLogHandler>();
        builder.Services.TryAddTransient<CorrelationIdHandler>();

        builder.AddHttpMessageHandler<OutboundLogHandler>()
            .AddHttpMessageHandler<CorrelationIdHandler>();

        return builder;
    }

    private static LoggerConfiguration ConfigureBaseLogging(this LoggerConfiguration loggerConfiguration, string appName)
    {
        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Async(a => a.Console(theme: AnsiConsoleTheme.Code))
            .Enrich.FromLogContext()
            .Enrich.WithProperty("appContext", appName ?? Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"))
            .Enrich.WithProperty("env", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        return loggerConfiguration;
    }

    private static LoggerConfiguration AddApplicationInsights(this LoggerConfiguration loggerConfiguration, LoggingOptions loggingOptions)
    {
        if (string.IsNullOrWhiteSpace(loggingOptions.AppInsightKey)) return loggerConfiguration;

        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        telemetryConfiguration.InstrumentationKey = loggingOptions.AppInsightKey;
        loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
        return loggerConfiguration;
    }

    private static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();
    }
}