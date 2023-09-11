using Microsoft.AspNetCore.Mvc;
using TelemetryApp.Core.ProjectServices.Repository;

namespace TelemetryApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectsController : Controller
{
    public ProjectsController(IProjectServiceRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<string[]>> ReadProjects([FromQuery] bool includeInactive = false)
    {
        return await repository.ReadAllProjectsAsync(includeInactive);
    }

    [HttpGet("{project}/Services")]
    public async Task<ActionResult<string[]>> ReadServices([FromRoute] string project, [FromQuery] bool includeInactive = false)
    {
        return await repository.ReadAllServicesAsync(project, includeInactive);
    }

    private readonly IProjectServiceRepository repository;
}