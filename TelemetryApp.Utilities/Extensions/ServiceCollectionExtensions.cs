using Microsoft.Extensions.DependencyInjection;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Api.Client.Projects;
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
        var restClient = RestClientBuilder.BuildRestClient(serviceUrl);
        // configure logger
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

        // configure read clients
        var logReaderClient = new LogReaderClient(restClient);
        services.AddSingleton<ILogReaderClient>(logReaderClient);

        var projectsReaderClient = new ProjectsClient(restClient);
        services.AddSingleton<IProjectsClient>(projectsReaderClient);

        return services;
    }
}