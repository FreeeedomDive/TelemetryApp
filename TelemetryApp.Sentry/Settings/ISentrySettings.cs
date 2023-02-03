namespace TelemetryApp.Sentry.Settings;

public interface ISentrySettings
{
    public string Dsn { get; init; }
}