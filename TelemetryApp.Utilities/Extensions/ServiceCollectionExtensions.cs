using Microsoft.Extensions.DependencyInjection;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Configuration;
using TelemetryApp.Utilities.Filters;

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

    public static IServiceCollection ConfigureApiTelemetryClient(this IServiceCollection services, string project, string service, Action<TelemetryFilter>? configureTelemetryFilters = null)
    {
        var restClient = RestClientBuilder.BuildRestClient();
        var apiTelemetryClient = new ApiTelemetryClient(restClient, project, service);
        services.AddSingleton<IApiTelemetryClient>(apiTelemetryClient);
        
        var filter = new TelemetryFilter();
        if (configureTelemetryFilters != null)
        {
            configureTelemetryFilters(filter);
        
            filter.ValidateRestrictions();
            filter.BuildFilters();
        }

        services.AddSingleton(filter);

        return services;
    }
}