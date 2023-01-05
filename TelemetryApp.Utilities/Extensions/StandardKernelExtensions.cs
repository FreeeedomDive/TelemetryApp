using Ninject;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Configuration;
using TelemetryApp.Utilities.Filters;

namespace TelemetryApp.Utilities.Extensions;

public static class StandardKernelExtensions
{
    public static StandardKernel ConfigureLoggerClient(this StandardKernel ninjectKernel, string project, string service)
    {
        var restClient = RestClientBuilder.BuildRestClient();
        var loggerClient = new LoggerClient(restClient, project, service);
        ninjectKernel.Bind<ILoggerClient>().ToConstant(loggerClient);

        return ninjectKernel;
    }

    public static StandardKernel ConfigureApiTelemetryClient(this StandardKernel ninjectKernel, string project, string service)
    {
        var restClient = RestClientBuilder.BuildRestClient();
        var apiTelemetryClient = new ApiTelemetryClient(restClient, project, service);
        ninjectKernel.Bind<IApiTelemetryClient>().ToConstant(apiTelemetryClient);

        return ninjectKernel;
    }

    public static StandardKernel ConfigureApiTelemetryFilters(this StandardKernel ninjectKernel, Action<TelemetryFilter> configureFilters)
    {
        var filter = new TelemetryFilter();
        configureFilters(filter);
        
        filter.ValidateRestrictions();
        filter.BuildFilters();

        ninjectKernel.Bind<TelemetryFilter>().ToConstant(filter);

        return ninjectKernel;
    }
}