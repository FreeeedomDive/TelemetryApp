using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace TelemetryApp.Core.ProjectServices.Repository;

public class ProjectServiceRepository : IProjectServiceRepository
{
    public ProjectServiceRepository(ISqlRepository<ProjectServiceStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync(string project, string service)
    {
        await sqlRepository.CreateAsync(new ProjectServiceStorageElement
        {
            Id = Guid.NewGuid(),
            Project = project,
            Service = service,
        });
    }

    public async Task<string[]> ReadAllProjectsAsync(bool includeInactive = false)
    {
        return await sqlRepository
            .BuildCustomQuery()
            .WhereIf(!includeInactive, x => !x.IsInactive)
            .Select(x => x.Project)
            .Distinct()
            .OrderBy(x => x)
            .ToArrayAsync();
    }

    public async Task<bool> IsProjectExistAsync(string project)
    {
        return await sqlRepository.BuildCustomQuery().AnyAsync(x => x.Project == project);
    }

    public async Task<string[]> ReadAllServicesAsync(string project, bool includeInactive = false)
    {
        return await sqlRepository
            .BuildCustomQuery()
            .Where(x => x.Project == project)
            .WhereIf(!includeInactive, x => !x.IsInactive)
            .Select(x => x.Service)
            .Distinct()
            .OrderBy(x => x)
            .ToArrayAsync();
    }

    public async Task<bool> IsServiceExistAsync(string project, string service)
    {
        return await sqlRepository.BuildCustomQuery().AnyAsync(x => x.Project == project && x.Service == service);
    }

    private readonly ISqlRepository<ProjectServiceStorageElement> sqlRepository;
}