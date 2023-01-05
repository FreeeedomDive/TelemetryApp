using Microsoft.Extensions.DependencyInjection;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Configuration;
using TelemetryApp.Utilities.Filters;

namespace TelemetryApp.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureTelemetryClientWithLogger(
        this IServiceCollection services,
        string project,
        string service,
        string? serviceUrl = null,
        Action<TelemetryFilter>? configureTelemetryFilters = null
    )
    {
        // configure logger
        var restClient = RestClientBuilder.BuildRestClient(serviceUrl);
        var loggerClient = new LoggerClient(restClient, project, service);
        services.AddSingleton<ILoggerClient>(loggerClient);

        // configure telemetry client
        var apiTelemetryClient = new ApiTelemetryClient(restClient, project, service);
        services.AddSingleton<IApiTelemetryClient>(apiTelemetryClient);

        // configure telemetry filters
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