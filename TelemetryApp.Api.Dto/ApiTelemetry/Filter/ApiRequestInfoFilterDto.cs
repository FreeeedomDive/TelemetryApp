namespace TelemetryApp.Api.Dto.ApiTelemetry.Filter;

public class ApiRequestInfoFilterDto
{
    public string? Project { get; set; }
    public string? Service { get; set; }
    public string? Method { get; set; }
    public string? Route { get; set; }
    public int? StatusCode { get; set; }
    public ExecutionTimeRange? ExecutionTimeRange { get; set; }
    public DateTimeRange? DateTimeRange { get; set; }
}