namespace TelemetryApp.Api.Dto.ApiTelemetry;

public class ApiTelemetryDto
{
    public string Project { get; set; }
    public string Service { get; set; }
    public string Method { get; set; }
    public string? Route { get; set; }
    public string? RoutePattern { get; set; }
    public Dictionary<string, string> RouteParametersValues { get; set; }
    public int StatusCode { get; set; }
    public long ExecutionTime { get; set; }
    public DateTime DateTime { get; set; }
}