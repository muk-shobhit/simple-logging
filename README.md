# Simple Serilog Logging #

Simple Serilog logging is a logging and monitoring service. It has integration with Serilog to provide structured logging and it enables traceability with minor configurations. Under the hood it leverages Microsoft.Extensions.Logging and Serilog to provide Structured logging and Azure App Insights integrations.

## Download ##

[Simple Serilog logging](https://www.nuget.org/packages/Simple.Serilog.Logging) is available using [nuget](https://www.nuget.org/packages/Simple.Serilog.Logging). To install Simple Logging, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

```Powershell
PM> Install-Package Simple.Serilog.Logging
```

## Getting Started

appsettings.json

```json
{
  "AppInsights": { "IKey": "<key>" }
}
```

program.cs
```csharp

var builder = WebApplication.CreateBuilder(args);
...

builder.Services.AddSimpleLogging();

//Optionally, register dependencies to log all outbound calls.
builder.Services.AddSimpleHttpClient<ICatFactClient, CatFactClient>();

//or you can add log handler to existing method
builder.Services.AddHttpClient<ICatFactClient,CatFactClient>().AddLogHandlers();

```

## .Net framework and dotnet core support?

The latest version targets .NET 6.

## Features ##

- Simple Serilog logging extension method that can be used in any dotnet core based apps
- App insight integration
- Structured and scoped logging - Serilog
- Tracking outbound calls. Such as request url, method, response code, error etc.
- CorrelationId middleware to track all events emitted from the service.
- Configurable logging
