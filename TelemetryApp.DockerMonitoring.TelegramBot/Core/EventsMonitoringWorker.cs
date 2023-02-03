using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelemetryApp.Api.Client.Log;
using Message = Docker.DotNet.Models.Message;

namespace TelemetryApp.DockerMonitoring.TelegramBot.Core;

public class EventsMonitoringWorker : IWorker
{
    public EventsMonitoringWorker(
        ITelegramBotClient telegramBotClient,
        IDockerClient dockerClient,
        ILoggerClient logger,
        Settings.Settings settings,
        CancellationTokenSource cancellationTokenSource
    )
    {
        this.telegramBotClient = telegramBotClient;
        this.dockerClient = dockerClient;
        this.logger = logger;
        this.settings = settings;
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public async Task Start()
    {
        var progress = new Progress<Message>();
        progress.ProgressChanged += async (sender, dockerMessage) =>
        {
            Console.WriteLine($"Received event\n{JsonConvert.SerializeObject(dockerMessage)}");
            if (dockerMessage.Type.ToLower() != "container")
            {
                return;
            }

            var containerName = dockerMessage.Actor.Attributes.TryGetValue("name", out var name)
                ? name
                : dockerMessage.From;
            var newStatus = dockerMessage.Status;
            var message = $"{containerName} status update: {newStatus}";
            await telegramBotClient.SendTextMessageAsync(new ChatId(settings.ChatId), message);
        };

        Console.WriteLine("Listening docker events...");
        await dockerClient.System.MonitorEventsAsync(
            new ContainerEventsParameters(),
            progress,
            cancellationTokenSource.Token
        );
    }

    private readonly ITelegramBotClient telegramBotClient;
    private readonly IDockerClient dockerClient;
    private readonly ILoggerClient logger;
    private readonly Settings.Settings settings;
    private readonly CancellationTokenSource cancellationTokenSource;
}