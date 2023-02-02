using Newtonsoft.Json;

namespace TelemetryApp.DockerMonitoring.TelegramBot.Settings;

public class SettingsProvider : ISettingsProvider
{
    public SettingsProvider()
    {
        var content = File.ReadAllText("Settings/appsettings.json");
        Settings = JsonConvert.DeserializeObject<Settings>(content)!;
    }

    public Settings Settings { get; }
}