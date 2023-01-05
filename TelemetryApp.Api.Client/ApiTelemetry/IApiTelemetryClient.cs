namespace TelemetryApp.Api.Client.ApiTelemetry;

public interface IApiTelemetryClient
{
    Task CreateAsync(string method, string routePattern, Dictionary<string, string> routeValues, int statusCode, long executionTime);
}