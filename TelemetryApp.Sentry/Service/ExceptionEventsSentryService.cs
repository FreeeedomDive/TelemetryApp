using Sentry;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Sentry.Settings;

namespace TelemetryApp.Sentry.Service;

public class ExceptionEventsSentryService : IExceptionEventsSentryService
{
    private readonly ISentrySettings settings;

    public ExceptionEventsSentryService(ISentrySettings settings)
    {
        this.settings = settings;
    }

    public void CaptureException(LogDto log)
    {
        using var _ = SentrySdk.Init(settings.Dsn);
        var @params = log.Params.Length > 0 ? string.Join(", ", log.Params) : "none";
        SentrySdk.CaptureMessage(
            $"Error in {log.Project}.{log.Service} - {log.Template}\nParams: {@params}\nException:\n{log.Exception}",
            SentryLevel.Error
        );
    }
}