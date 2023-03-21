namespace TelemetryApp.Alerts.TelegramBot.Settings;

public class Settings
{
    public string TelegramToken { get; set; }
    public string TelemetryAppUrl { get; set; }
    public string DockerApiUrl { get; set; }
    public long ChatId { get; set; }
}