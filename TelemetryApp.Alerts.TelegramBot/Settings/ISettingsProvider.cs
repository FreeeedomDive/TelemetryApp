namespace TelemetryApp.Alerts.TelegramBot.Settings;

public interface ISettingsProvider
{
    public Settings Settings { get; }
}