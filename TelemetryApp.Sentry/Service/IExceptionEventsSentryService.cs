using TelemetryApp.Api.Dto.Logs;

namespace TelemetryApp.Sentry.Service;

public interface IExceptionEventsSentryService
{
    void CaptureException(LogDto log);
}