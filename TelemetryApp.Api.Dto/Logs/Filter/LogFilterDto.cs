namespace TelemetryApp.Api.Dto.Logs.Filter;

public class LogFilterDto
{
    public string? Project { get; set; }
    public string? Service { get; set; }
    public string? LoggerName { get; set; }
    public string? LogLevel { get; set; }
    public string? Template { get; set; }
    public DateTimeRange? DateTimeRange { get; set; }
}