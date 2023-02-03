using Newtonsoft.Json;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;
using TelemetryApp.Core.Logs.Repository;
using TelemetryApp.Core.ProjectServices.Repository;
using TelemetryApp.Sentry;
using TelemetryApp.Sentry.Service;

namespace TelemetryApp.Core.Logs.Service;

public class LogService : ILogService
{
    public LogService(
        IProjectServiceRepository projectServiceRepository,
        ILogRepository logRepository,
        IExceptionEventsSentryService exceptionEventsSentryService
    )
    {
        this.projectServiceRepository = projectServiceRepository;
        this.logRepository = logRepository;
        this.exceptionEventsSentryService = exceptionEventsSentryService;
    }

    public async Task CreateAsync(LogDto logDto)
    {
        if (
            !await projectServiceRepository.IsProjectExistAsync(logDto.Project)
            || !await projectServiceRepository.IsServiceExistAsync(logDto.Project, logDto.Service)
        )
        {
            await projectServiceRepository.CreateAsync(logDto.Project, logDto.Service);
        }

        await logRepository.CreateAsync(logDto);

        if (string.IsNullOrEmpty(logDto.Exception)) return;
        var exception = JsonConvert.DeserializeObject<Exception>(logDto.Exception);
        if (exception == null) return;

        exceptionEventsSentryService.CaptureException(exception);
    }

    public async Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        return await logRepository.FindAsync(filter);
    }

    private readonly IProjectServiceRepository projectServiceRepository;
    private readonly ILogRepository logRepository;
    private readonly IExceptionEventsSentryService exceptionEventsSentryService;
}