namespace TelemetryApp.Core.Elastic;

public class ElasticInsertValueException : Exception
{
    public ElasticInsertValueException(string message) : base(message)
    {
    }
}