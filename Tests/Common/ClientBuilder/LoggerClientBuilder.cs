using TelemetryApp.Api.Client.Log;

namespace Common.ClientBuilder;

public class LoggerClientBuilder
{
    public static ILoggerClient Build(string project, string service)
    {
        return new LoggerClient(RestClientBuilder.BuildRestClient("https://localhost:22222"), project, service);
    }
}