using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Api.Client.Log;

public interface ILogReaderClient
{
    Task<LogDto[]> FindAsync(LogFilterDto filter);
}