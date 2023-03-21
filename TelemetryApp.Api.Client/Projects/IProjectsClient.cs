namespace TelemetryApp.Api.Client.Projects;

public interface IProjectsClient
{
    Task<string[]> ReadProjectsAsync();
    Task<string[]> ReadServicesAsync(string project);
}