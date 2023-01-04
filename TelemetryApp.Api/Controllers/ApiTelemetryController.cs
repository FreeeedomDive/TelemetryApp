using Microsoft.AspNetCore.Mvc;
using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;
using TelemetryApp.Core.ApiTelemetry.Service;

namespace TelemetryApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiTelemetryController : Controller
{
    public ApiTelemetryController(IApiTelemetryService apiTelemetryService)
    {
        this.apiTelemetryService = apiTelemetryService;
    }

    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] ApiTelemetryDto apiTelemetryDto)
    {
        await apiTelemetryService.CreateAsync(apiTelemetryDto);
        return NoContent();
    }

    [HttpPost("find")]
    public async Task<ActionResult<ApiTelemetryDto[]>> Find([FromBody] ApiRequestInfoFilterDto filter)
    {
        return await apiTelemetryService.FindAsync(filter);
    }

    private readonly IApiTelemetryService apiTelemetryService;
}