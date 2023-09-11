using RestSharp;
using TelemetryApp.Api.Client.Extensions;

namespace TelemetryApp.Api.Client.Projects;

public class ProjectsClient : IProjectsClient
{
    public ProjectsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<string[]> ReadProjectsAsync(bool includeInactive = false)
    {
        var request = new RestRequest("projects").AddQueryParameter("includeInactive", includeInactive);
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<string[]>();
    }

    public async Task<string[]> ReadServicesAsync(string project, bool includeInactive = false)
    {
        var request = new RestRequest($"projects/{project}/services").AddQueryParameter("includeInactive", includeInactive);
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<string[]>();
    }

    private readonly RestClient restClient;
}