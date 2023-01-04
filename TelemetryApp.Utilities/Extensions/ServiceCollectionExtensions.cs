using Microsoft.Extensions.DependencyInjection;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Configuration;

namespace TelemetryApp.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureLoggerClient(this IServiceCollection services, string project, string service)
    {
        var restClient = RestClientBuilder.BuildRestClient();
        var loggerClient = new LoggerClient(restClient, project, service);
        services.AddSingleton<ILoggerClient>(loggerClient);

        return services;
    }

    public static IServiceCollection ConfigureApiTelemetryClient(this IServiceCollection services, string project, string service)
    {
        var restClient = RestClientBuilder.BuildRestClient();
        var apiTelemetryClient = new ApiTelemetryClient(restClient, project, service);
        services.AddSingleton<IApiTelemetryClient>(apiTelemetryClient);

        return services;
    }
}