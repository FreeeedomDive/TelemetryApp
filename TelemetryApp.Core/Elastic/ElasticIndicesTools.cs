namespace TelemetryApp.Core.Elastic;

public static class ElasticIndicesTools
{
    public static string CreateIndexName(string indexName, string application, string environment)
    {
        return $"{application}_{indexName}_{environment}".ToLower();
    }
}