namespace TelemetryApp.Api.Client.Log;

public interface ILoggerClient
{
    Task DebugAsync(string template, params object[] args);
    Task InfoAsync(string template, params object[] args);
    Task WarnAsync(string template, params object[] args);
    Task WarnAsync(Exception exception, string template, params object[] args);
    Task ErrorAsync(string template, params object[] args);
    Task ErrorAsync(Exception exception, string template, params object[] args);
}