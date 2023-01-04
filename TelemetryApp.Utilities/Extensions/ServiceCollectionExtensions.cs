using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Configuration;

namespace TelemetryApp.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureLoggerClient(this IServiceCollection services, string project, string service)
    {
        var restClient = BuildRestClient();
        var loggerClient = new LoggerClient(restClient, project, service);
        services.AddSingleton<ILoggerClient>(loggerClient);

        return services;
    }

    public static IServiceCollection ConfigureApiTelemetryClient(this IServiceCollection services, string project, string service)
    {
        var restClient = BuildRestClient();
        var apiTelemetryClient = new ApiTelemetryClient(restClient, project, service);
        services.AddSingleton<IApiTelemetryClient>(apiTelemetryClient);

        return services;
    }

    private static RestClient BuildRestClient()
    {
        return RestClientBuilder.BuildRestClient("https://localhost:6651", false);
    }
}