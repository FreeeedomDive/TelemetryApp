namespace TelemetryApp.Core.Elastic;

public class ElasticOptions
{
    public string ConnectionString { get; set; }
    public string ConnectionUserName { get; set; }
    public string ConnectionPassword { get; set; }
    public string ApplicationName { get; set; }
    public string Environment { get; set; }
}