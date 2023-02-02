﻿using Docker.DotNet;
using Telegram.Bot;
using TelemetryApp.DockerMonitoring.TelegramBot.Core;
using TelemetryApp.DockerMonitoring.TelegramBot.Settings;
using TelemetryApp.Utilities.Configuration;

namespace TelemetryApp.DockerMonitoring.TelegramBot;

public class Program
{
    public static async Task Main()
    {
        var settings = new SettingsProvider().Settings;
        var telegramBotClient = new TelegramBotClient(settings.TelegramToken);
        var dockerClient = new DockerClientConfiguration(new Uri(settings.DockerApiUrl)).CreateClient();
        var loggerClient = ClientsBuilder.BuildLoggerClient(
            "TelemetryApp",
            "DockerMonitoringTelegramBot",
            settings.TelemetryAppUrl
        );
        var cancellationTokenSource = new CancellationTokenSource();
        IWorker[] workers =
        {
            new TelegramMessagesWorker(telegramBotClient, dockerClient, loggerClient, cancellationTokenSource),
            new EventsMonitoringWorker(telegramBotClient, dockerClient, loggerClient, settings, cancellationTokenSource)
        };
        await Task.WhenAll(workers.Select(x => x.Start()));
    }
}