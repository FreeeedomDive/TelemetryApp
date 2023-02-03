namespace TelemetryApp.DockerMonitoring.TelegramBot.Settings;

public interface ISettingsProvider
{
    public Settings Settings { get; }
}