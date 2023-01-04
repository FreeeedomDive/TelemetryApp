namespace TelemetryApp.Core.ProjectServices.Repository;

public interface IProjectServiceRepository
{
    Task CreateAsync(string project, string service);
    
    Task<string[]> ReadAllProjectsAsync();
    Task<bool> IsProjectExistAsync(string project);

    Task<string[]> ReadAllServicesAsync(string project);
    Task<bool> IsServiceExistAsync(string project, string service);
}