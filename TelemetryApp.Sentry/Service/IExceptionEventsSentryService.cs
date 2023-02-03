namespace TelemetryApp.Sentry.Service;

public interface IExceptionEventsSentryService
{
    void CaptureException(Exception exception);
}