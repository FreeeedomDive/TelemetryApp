using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
                GetRoutePattern(context),
                GetRouteParams(context),
                context.Response?.StatusCode ?? 0,
                stopwatch.ElapsedMilliseconds
            );
        }
    }

    private static string GetRoutePattern(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is not RouteEndpoint routeEndpoint)
        {
            return context.Request?.Path.Value ?? "";
        }

        return routeEndpoint.RoutePattern.RawText ?? "";
    }

    private static Dictionary<string, string> GetRouteParams(HttpContext context)
    {
        var routeData = context.GetRouteData();
        return routeData.Values.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? "");
    }

    private readonly RequestDelegate next;
}