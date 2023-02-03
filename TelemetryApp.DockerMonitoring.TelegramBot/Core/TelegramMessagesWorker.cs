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
                    var containers = await dockerClient.Containers.ListContainersAsync(
                        new ContainersListParameters
                        {
                            All = true
                        },
                        cancellationToken
                    );
                    var groups = containers
                        .GroupBy(container => container.Names.First()[1..].Split('-')[0]);
                    var applications = groups
                        .Select(group =>
                            $"{group.Key}\n{group.Select(container => $"\t{container.Names.First()[1..].Split('-')[1]}")}")
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