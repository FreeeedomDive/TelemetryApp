using Ninject;
using RestSharp;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Utilities.Configuration;

namespace TelemetryApp.Utilities.Extensions;

public static class StandardKernelExtensions
{
    public static StandardKernel ConfigureLoggerClient(this StandardKernel ninjectKernel, string project, string service)
    {
        var restClient = BuildRestClient();
        var loggerClient = new LoggerClient(restClient, project, service);
        ninjectKernel.Bind<ILoggerClient>().ToConstant(loggerClient);

        return ninjectKernel;
    }

    public static StandardKernel ConfigureApiTelemetryClient(this StandardKernel ninjectKernel, string project, string service)
    {
        var restClient = BuildRestClient();
        var apiTelemetryClient = new ApiTelemetryClient(restClient, project, service);
        ninjectKernel.Bind<IApiTelemetryClient>().ToConstant(apiTelemetryClient);

        return ninjectKernel;
    }

    private static RestClient BuildRestClient()
    {
        return RestClientBuilder.BuildRestClient("https://localhost:6651", false);
    }
}