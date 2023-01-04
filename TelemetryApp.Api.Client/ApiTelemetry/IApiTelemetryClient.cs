namespace TelemetryApp.Api.Client.ApiTelemetry;

public interface IApiTelemetryClient
{
    Task CreateAsync(string method, string route, int statusCode, long executionTime);
}