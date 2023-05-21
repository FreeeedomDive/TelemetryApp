namespace TelemetryApp.Core.Elastic;

public class ElasticReadValuesException : Exception
{
    public ElasticReadValuesException(string message) : base(message)
    {
    }
}