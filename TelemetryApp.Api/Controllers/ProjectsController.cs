using Microsoft.AspNetCore.Mvc;
using TelemetryApp.Core.ProjectServices.Repository;

namespace TelemetryApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectsController
{
    public ProjectsController(IProjectServiceRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<string[]>> ReadProjects()
    {
        return await repository.ReadAllProjectsAsync();
    }

    [HttpGet("{project}/Services")]
    public async Task<ActionResult<string[]>> ReadServices(string project)
    {
        return await repository.ReadAllServicesAsync(project);
    }

    private readonly IProjectServiceRepository repository;
}