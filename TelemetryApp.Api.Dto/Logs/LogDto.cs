namespace TelemetryApp.Api.Dto.Logs;

public class LogDto
{
    public string Project { get; set; }
    public string Service { get; set; }
    public string LogLevel { get; set; }
    public string Template { get; set; }
    public string[] Params { get; set; }
    public Exception? Exception { get; set; }
    public DateTime DateTime { get; set; }
}