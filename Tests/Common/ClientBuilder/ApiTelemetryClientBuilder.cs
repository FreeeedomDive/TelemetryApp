using TelemetryApp.Api.Client.ApiTelemetry;

namespace Common.ClientBuilder;

public static class ApiTelemetryClientBuilder
{
    public static IApiTelemetryClient Build(string project, string service)
    {
        return new ApiTelemetryClient(RestClientBuilder.BuildRestClient("https://localhost:22222"), project, service, false);
    }
}