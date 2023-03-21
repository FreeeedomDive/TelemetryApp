using RestSharp;
using TelemetryApp.Api.Client.Extensions;

namespace TelemetryApp.Api.Client.Projects;

public class ProjectsClient : IProjectsClient
{
    public ProjectsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<string[]> ReadProjectsAsync()
    {
        var request = new RestRequest("projects");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<string[]>();
    }

    public async Task<string[]> ReadServicesAsync(string project)
    {
        var request = new RestRequest($"projects/{project}/services");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<string[]>();
    }

    private readonly RestClient restClient;
}