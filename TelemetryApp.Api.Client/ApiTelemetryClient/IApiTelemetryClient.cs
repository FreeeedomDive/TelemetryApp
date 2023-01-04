namespace TelemetryApp.Api.Client.ApiTelemetryClient;

public interface IApiTelemetryClient
{
    Task CreateAsync(string method, string route, int statusCode, long executionTime);
}