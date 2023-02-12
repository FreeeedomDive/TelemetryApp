using Docker.DotNet;
using Docker.DotNet.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelemetryApp.Api.Client.Log;

namespace TelemetryApp.DockerMonitoring.TelegramBot.Core;

public class TelegramMessagesWorker : IWorker
{
    public TelegramMessagesWorker(
        ITelegramBotClient telegramBotClient,
        IDockerClient dockerClient,
        ILoggerClient logger,
        CancellationTokenSource cancellationTokenSource
    )
    {
        this.telegramBotClient = telegramBotClient;
        this.dockerClient = dockerClient;
        this.logger = logger;
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public async Task Start()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        telegramBotClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationTokenSource.Token
        );

        Console.WriteLine("Starting bot...");
        await Task.Delay(-1, cancellationTokenSource.Token);
    }

    private async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cts)
    {
        await logger.ErrorAsync(exception, "Telegram polling error");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } messageText } message)
            return;

        var chatId = message.Chat.Id;
        await logger.InfoAsync("{Username} ({UserId}): {Message}", message.Chat.Username ?? chatId.ToString(), chatId,
            messageText);
        try
        {
            switch (messageText)
            {
                case "/start":
                    await SendMessage(chatId, "Start");
                    return;
                case "/status":
                {
                    var containers = (await dockerClient.Containers.ListContainersAsync(
                        new ContainersListParameters
                        {
                            All = true
                        },
                        cancellationToken
                    )).ToArray();
                    var otherContainers = containers
                        .Where(container => container.Names.First().Split('-', '_').Length == 1)
                        .ToArray();
                    containers = containers.Except(otherContainers).ToArray();
                    var oldComposeContainers = containers
                        .Where(container => !container.Names.First().StartsWith("/k8s"));
                    var newKubernetesContainers = containers
                        .Where(container => container.Names.First().StartsWith("/k8s"));
                    var applications = BuildCommonApplicationContainersOutput(otherContainers)
                        .Concat(BuildComposeApplicationContainersOutput(oldComposeContainers))
                        .Concat(BuildKubernetesApplicationContainersOutput(newKubernetesContainers))
                        .ToArray();

                    var content = string.Join("\n", applications);
                    await SendMessage(chatId, content);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            await logger.ErrorAsync(e, "Exception in message handler");
            await SendMessage(chatId, $"Error\n{e.Message}");
        }
    }

    private static IEnumerable<string> BuildCommonApplicationContainersOutput(
        IEnumerable<ContainerListResponse> containers)
    {
        return containers.Select(c => $"{c.Names.First()[1..]}  -  {c.Status}");
    }

    private static IEnumerable<string> BuildComposeApplicationContainersOutput(
        IEnumerable<ContainerListResponse> containers)
    {
        var groups = containers
            .GroupBy(container => container.Names.First()[1..].Split('-')[0]);
        return groups
            .Select(group =>
            {
                var app = group.Key;
                var containerNames = group
                    .Select(container => $"    {container.Names.First()[1..].Split('-')[1]}  -  {container.Status}")
                    .ToArray();
                return $"{app}\n{string.Join("\n", containerNames)}";
            })
            .ToArray();
    }

    private static IEnumerable<string> BuildKubernetesApplicationContainersOutput(
        IEnumerable<ContainerListResponse> containers)
    {
        var groups = containers
            .GroupBy(container => container.Names.First().Split('_')[2].Split('-')[0]);
        return groups
            .Select(group =>
            {
                var app = group.Key;
                var containerNames = group
                    .Select(container =>
                        $"    {container.Names.First().Split('_')[2].Split('-')[1]}  -  {container.Status}")
                    .ToArray();
                return $"{app}\n{string.Join("\n", containerNames)}";
            })
            .ToArray();
    }

    private async Task SendMessage(long chatId, string message)
    {
        try
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                cancellationToken: cancellationTokenSource.Token);
        }
        catch (Exception exception)
        {
            await logger.ErrorAsync(exception, "Exception in SendMessage");
        }
    }

    private readonly ITelegramBotClient telegramBotClient;
    private readonly IDockerClient dockerClient;
    private readonly ILoggerClient logger;
    private readonly CancellationTokenSource cancellationTokenSource;
}