namespace TelemetryApp.DockerMonitoring.TelegramBot.Core;

public interface IWorker
{
    Task Start();
}