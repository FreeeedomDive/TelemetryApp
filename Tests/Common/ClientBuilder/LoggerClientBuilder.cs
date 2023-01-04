using TelemetryApp.Api.Client.Log;

namespace Common.ClientBuilder;

public class LoggerClientBuilder
{
    public static ILoggerClient Build()
    {
        return new LoggerClient(RestClientBuilder.BuildRestClient("https://localhost:6651"), "Tests", "Tests");
    }
    
}