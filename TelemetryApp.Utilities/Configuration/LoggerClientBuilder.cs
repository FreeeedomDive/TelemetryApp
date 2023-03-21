using TelemetryApp.Api.Client.Log;
using TelemetryApp.Api.Client.Projects;

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

    public static ILogReaderClient BuildLogReaderClient(string? serviceUrl = null)
    {
        var restClient = RestClientBuilder.BuildRestClient(serviceUrl);
        return new LogReaderClient(restClient);
    }

    public static IProjectsClient BuildProjectsClient(string? serviceUrl = null)
    {
        var restClient = RestClientBuilder.BuildRestClient(serviceUrl);
        return new ProjectsClient(restClient);
    }
}