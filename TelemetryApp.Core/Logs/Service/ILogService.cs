using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Core.Logs.Service;

public interface ILogService
{
    Task CreateAsync(LogDto logDto);
    Task<LogDto[]> FindAsync(LogFilterDto filter);
}