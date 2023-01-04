using TelemetryApp.Api.Client.ApiTelemetry;

namespace Common.ClientBuilder;

public static class ApiTelemetryClientBuilder
{
    public static IApiTelemetryClient Build()
    {
        return new ApiTelemetryClient(RestClientBuilder.BuildRestClient("https://localhost:6651"), "Tests", "Tests");
    }
}