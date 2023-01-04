using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;

namespace TelemetryApp.Utilities.Middlewares;

public class RequestLoggingMiddleware
{
    public RequestLoggingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, IApiTelemetryClient apiTelemetryClient, ILoggerClient loggerClient)
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            await next(context);
        }
        catch (Exception e)
        {
            await loggerClient.ErrorAsync(e, "Unhandled exception in api method {method}", context.Request?.Method ?? "");
        }
        finally
        {
            await apiTelemetryClient.CreateAsync(
                context.Request?.Method ?? "",
                context.Request?.Path.Value ?? "",
                context.Response?.StatusCode ?? 0,
                stopwatch.ElapsedMilliseconds
            );
        }
    }

    private readonly RequestDelegate next;
}