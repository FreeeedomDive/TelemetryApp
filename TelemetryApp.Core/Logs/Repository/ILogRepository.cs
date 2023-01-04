using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Core.Logs.Repository;

public interface ILogRepository
{
    Task CreateAsync(LogDto logDto);
    Task<LogDto[]> FindAsync(LogFilterDto filter);
}