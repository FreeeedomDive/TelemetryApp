using RestSharp;
using TelemetryApp.Api.Client.Extensions;
using TelemetryApp.Api.Dto.ApiTelemetry;

namespace TelemetryApp.Api.Client.ApiTelemetry;

public class ApiTelemetryClient : IApiTelemetryClient
{
    public ApiTelemetryClient(RestClient restClient, string project, string service)
    {
        this.restClient = restClient;
        this.project = project;
        this.service = service;
    }

    public async Task CreateAsync(string method, string route, int statusCode, long executionTime)
    {
        var request = new RestRequest("ApiTelemetry/create");
        request.AddJsonBody(new ApiTelemetryDto
        {
            Project = project,
            Service = service,
            Method = method,
            Route = route,
            StatusCode = statusCode,
            ExecutionTime = executionTime
        });
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }
    
    private readonly RestClient restClient;
    private readonly string project;
    private readonly string service;
}