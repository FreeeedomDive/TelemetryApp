using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelemetryApp.Api.Client.Log;
using TelemetryApp.Api.Client.Projects;
using TelemetryApp.Api.Dto;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Alerts.TelegramBot.Core;

public class ErrorAlertsWorker : IWorker
{
    public ErrorAlertsWorker(
        ITelegramBotClient telegramBotClient,
        IProjectsClient projectsClient,
        ILogReaderClient logReaderClient,
        Settings.Settings settings,
        CancellationTokenSource cancellationTokenSource)
    {
        this.projectsClient = projectsClient;
        this.logReaderClient = logReaderClient;
        this.telegramBotClient = telegramBotClient;
        this.settings = settings;
        this.cancellationTokenSource = cancellationTokenSource;
        timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
    }

    public Task Start()
    {
        workerTask = StartCheckErrorsAsync();
        return Task.CompletedTask;
    }

    private async Task StartCheckErrorsAsync()
    {
        var start = DateTime.UtcNow;
        while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
        {
            var end = DateTime.UtcNow;
            var startDateTimeFormatted = start.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
            var endDateTimeFormatted = end.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"Getting errors from {startDateTimeFormatted} to {endDateTimeFormatted}");
            try
            {
                var projects = await projectsClient.ReadProjectsAsync();
                var messageBuilder = new StringBuilder().Append($"Errors from {startDateTimeFormatted} to {endDateTimeFormatted}").AppendLine();
                foreach (var project in projects)
                {
                    var errors = await logReaderClient.FindAsync(new LogFilterDto
                    {
                        Project = project,
                        LogLevel = "ERROR",
                        DateTimeRange = new DateTimeRange
                        {
                            From = start,
                            To = end,
                        },
                    });
                    if (errors.Length == 0)
                    {
                        continue;
                    }
                    messageBuilder.Append(project).Append(": ").Append(errors.Length).AppendLine();
                }

                await telegramBotClient.SendTextMessageAsync(new ChatId(settings.ChatId), messageBuilder.ToString());
            }
            catch
            {
                Console.WriteLine($"Iteration at {end} has failed");
            }
            start = end;
        }
    }

    private readonly IProjectsClient projectsClient;
    private readonly ILogReaderClient logReaderClient;
    private readonly ITelegramBotClient telegramBotClient;
    private readonly Settings.Settings settings;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly PeriodicTimer timer;
    private Task workerTask;
}