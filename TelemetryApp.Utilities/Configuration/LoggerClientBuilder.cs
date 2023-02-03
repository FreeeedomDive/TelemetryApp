using TelemetryApp.Api.Client.Log;

namespace TelemetryApp.Utilities.Configuration;

public static class ClientsBuilder
{
    public static ILoggerClient BuildLoggerClient(
        string project,
        string service,
        string? serviceUrl = null
    )
    {
        var restClient = RestClientBuilder.BuildRestClient(serviceUrl);
        return new LoggerClient(restClient, project, service);
    }
}