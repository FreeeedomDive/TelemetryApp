using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TelemetryApp.Api.Client.Configuration;
using TelemetryApp.Api.Client.Log;

namespace TelemetryApp.Api.Client.Extensions;

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
        var loggerClient = new LoggerClient(restClient, project, service);
        services.AddSingleton<ILoggerClient>(loggerClient);

        return services;
    }

    private static RestClient BuildRestClient()
    {
        return RestClientBuilder.BuildRestClient("https://localhost:6651", false);
    }
}