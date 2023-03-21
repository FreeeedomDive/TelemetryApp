using RestSharp;
using TelemetryApp.Api.Client.Extensions;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Api.Client.Log;

public class LogReaderClient : ILogReaderClient
{
    public LogReaderClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        var request = new RestRequest("logs/find");
        request.AddJsonBody(filter);
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<LogDto[]>();
    }

    private readonly RestClient restClient;
}