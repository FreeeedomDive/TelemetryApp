using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;
using TelemetryApp.Core.Logs.Repository;
using TelemetryApp.Core.ProjectServices.Repository;

namespace TelemetryApp.Core.Logs.Service;

public class LogService : ILogService
{
    public LogService(
        IProjectServiceRepository projectServiceRepository,
        ILogRepository logRepository
    )
    {
        this.projectServiceRepository = projectServiceRepository;
        this.logRepository = logRepository;
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
    }

    public async Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        return await logRepository.FindAsync(filter);
    }

    private readonly IProjectServiceRepository projectServiceRepository;
    private readonly ILogRepository logRepository;
}