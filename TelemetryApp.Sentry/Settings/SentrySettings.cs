namespace TelemetryApp.Sentry.Settings;

public class SentrySettings : ISentrySettings
{
    public string Dsn { get; init; }
}