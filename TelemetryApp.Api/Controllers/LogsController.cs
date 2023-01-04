using Microsoft.AspNetCore.Mvc;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;
using TelemetryApp.Core.Logs.Service;

namespace TelemetryApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController : Controller
{
    public LogsController(ILogService logService)
    {
        this.logService = logService;
    }
    
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] LogDto logDto)
    {
        await logService.CreateAsync(logDto);
        return NoContent();
    }

    [HttpPost("find")]
    public async Task<ActionResult<LogDto[]>> Find([FromBody] LogFilterDto filter)
    {
        return await logService.FindAsync(filter);
    }
    
    private readonly ILogService logService;
}