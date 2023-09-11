using RestSharp;
using TelemetryApp.Api.Client.Extensions;
using TelemetryApp.Api.Dto.ApiTelemetry;

namespace TelemetryApp.Api.Client.ApiTelemetry;

public class ApiTelemetryClient : BaseClient, IApiTelemetryClient
{
    public ApiTelemetryClient(RestClient restClient, string project, string service, bool exceptionsSafe = true) : base(exceptionsSafe)
    {
        this.restClient = restClient;
        this.project = project;
        this.service = service;
    }

    public async Task CreateAsync(string method, string routePattern, Dictionary<string, string> routeValues, int statusCode, long executionTime)
    {
        await PerformRequestAsync(
            async () =>
            {
                var request = new RestRequest("ApiTelemetry/create");
                request.AddJsonBody(
                    new ApiTelemetryDto
                    {
                        Project = project,
                        Service = service,
                        Method = method,
                        RoutePattern = routePattern,
                        RouteParametersValues = routeValues,
                        StatusCode = statusCode,
                        ExecutionTime = executionTime,
                    }
                );
                var response = await restClient.ExecutePostAsync(request);
                response.ThrowIfNotSuccessful();
            }
        );
    }

    private readonly string project;

    private readonly RestClient restClient;
    private readonly string service;
}