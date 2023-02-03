namespace FromLogsToSentryExceptionsConverter.Tools;

public interface ITool
{
    Task ExecuteAsync(string[]? args = null);
}