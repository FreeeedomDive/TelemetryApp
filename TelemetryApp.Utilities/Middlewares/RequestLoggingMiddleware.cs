using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Filters;

namespace TelemetryApp.Utilities.Middlewares;

public class RequestLoggingMiddleware
{
    public RequestLoggingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, IApiTelemetryClient apiTelemetryClient, ILoggerClient loggerClient, TelemetryFilter filters)
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
            var method = context.Request?.Method ?? "";
            var route = GetRoutePattern(context) ?? "";
            var routeParams = GetRouteParams(context);
            var statusCode = context.Response?.StatusCode ?? 0;
            var elapsed = stopwatch.ElapsedMilliseconds;
            if (!filters.Filter(method, route))
            {
                await apiTelemetryClient.CreateAsync(method, route, routeParams, statusCode, elapsed);
            }
        }
    }

    private static string? GetRoutePattern(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint is not RouteEndpoint routeEndpoint
            ? context.Request?.Path.Value
            : routeEndpoint.RoutePattern.RawText;
    }

    private static Dictionary<string, string> GetRouteParams(HttpContext context)
    {
        var routeData = context.GetRouteData();
        return routeData.Values.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? "");
    }

    private readonly RequestDelegate next;
}