using RestSharp;
using TelemetryApp.Api.Client.Extensions;
using TelemetryApp.Api.Dto.Logs;

namespace TelemetryApp.Api.Client.Log;

public class LoggerClient : ILoggerClient
{
    public LoggerClient(RestClient restClient, string project, string service)
    {
        this.restClient = restClient;
        this.project = project;
        this.service = service;
    }

    public async Task DebugAsync(string template, params object[] args)
    {
        await CreateAsync("DEBUG", template, args);
    }

    public async Task InfoAsync(string template, params object[] args)
    {
        await CreateAsync("INFO", template, args);
    }

    public async Task WarnAsync(string template, params object[] args)
    {
        await CreateAsync("WARN", template, args);
    }

    public async Task WarnAsync(Exception exception, string template, params object[] args)
    {
        await CreateAsync("WARN", template, args, exception);
    }

    public async Task ErrorAsync(string template, params object[] args)
    {
        await CreateAsync("ERROR", template, args);
    }

    public async Task ErrorAsync(Exception exception, string template, params object[] args)
    {
        await CreateAsync("ERROR", template, args, exception);
    }

    private async Task CreateAsync(string level, string template, IEnumerable<object> args, Exception? exception = null)
    {
        try
        {
            var request = new RestRequest("Logs/create");
            request.AddJsonBody(new LogDto
            {
                Project = project,
                Service = service,
                LogLevel = level,
                Template = template,
                Params = args.Select(x => x == null ? "" : x.ToString()!).ToArray(),
                Exception = exception
            });
            var response = await restClient.ExecutePostAsync(request);
            response.ThrowIfNotSuccessful();
        }
        catch
        {
            // just prevent other apps from dying if telemetry api is dead
        }
    }

    private readonly RestClient restClient;
    private readonly string project;
    private readonly string service;
}