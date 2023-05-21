using TelemetryApp.Core.Elastic;

namespace TelemetryApp.Core.Logs.Repository.Elastic;

[ElasticStorageElement(nameof(ElasticLogStorageElement))]
public class ElasticLogStorageElement
{
    public Guid Id { get; set; }
    public string Project { get; set; }
    public string Service { get; set; }
    public string LogLevel { get; set; }
    public string Template { get; set; }
    public string[] Params { get; set; }
    public string Exception { get; set; }
    public DateTime DateTime { get; set; }
}