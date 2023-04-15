namespace TelemetryApp.Core.Elastic;

public class ElasticStorageElementAttribute : Attribute
{
    public ElasticStorageElementAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}