using Sentry;
using TelemetryApp.Sentry.Settings;

namespace TelemetryApp.Sentry.Service;

public class ExceptionEventsSentryService : IExceptionEventsSentryService
{
    private readonly ISentrySettings settings;

    public ExceptionEventsSentryService(ISentrySettings settings)
    {
        this.settings = settings;
    }

    public void CaptureException(Exception exception)
    {
        using var _ = SentrySdk.Init(settings.Dsn);
        SentrySdk.CaptureException(exception);
    }
}