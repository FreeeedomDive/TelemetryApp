namespace TelemetryApp.Api.Client.Projects;

public interface IProjectsClient
{
    Task<string[]> ReadProjectsAsync(bool includeInactive = false);
    Task<string[]> ReadServicesAsync(string project, bool includeInactive = false);
}