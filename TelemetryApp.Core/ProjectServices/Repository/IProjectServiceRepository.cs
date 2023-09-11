namespace TelemetryApp.Core.ProjectServices.Repository;

public interface IProjectServiceRepository
{
    Task CreateAsync(string project, string service);

    Task<string[]> ReadAllProjectsAsync(bool includeInactive = false);
    Task<bool> IsProjectExistAsync(string project);

    Task<string[]> ReadAllServicesAsync(string project, bool includeInactive = false);
    Task<bool> IsServiceExistAsync(string project, string service);
}