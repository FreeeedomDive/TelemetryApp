using Ninject;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Configuration;
using TelemetryApp.Utilities.Filters;

namespace TelemetryApp.Utilities.Extensions;

public static class StandardKernelExtensions
{
    public static StandardKernel ConfigureTelemetryClientWithLogger(
        this StandardKernel ninjectKernel,
        string project,
        string service,
        string? serviceUrl = null,
        Action<TelemetryFilter>? configureTelemetryFilters = null
    )
    {
        // configure logger
        var restClient = RestClientBuilder.BuildRestClient(serviceUrl);
        var loggerClient = new LoggerClient(restClient, project, service);
        ninjectKernel.Bind<ILoggerClient>().ToConstant(loggerClient);
        
        // configure telemetry client
        var apiTelemetryClient = new ApiTelemetryClient(restClient, project, service);
        ninjectKernel.Bind<IApiTelemetryClient>().ToConstant(apiTelemetryClient);
        
        // configure telemetry filters
        var filter = new TelemetryFilter();
        if (configureTelemetryFilters != null)
        {
            configureTelemetryFilters(filter);

            filter.ValidateRestrictions();
            filter.BuildFilters();
        }

        ninjectKernel.Bind<TelemetryFilter>().ToConstant(filter);

        return ninjectKernel;
    }
}