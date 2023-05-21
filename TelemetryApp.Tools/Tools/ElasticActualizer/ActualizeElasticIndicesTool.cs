using System.Reflection;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using TelemetryApp.Core.Elastic;

namespace TelemetryApp.Tools.Tools.ElasticActualizer;

public class ActualizeElasticIndicesTool : ITool
{
    public ActualizeElasticIndicesTool(
        ElasticsearchClient elasticsearchClient,
        ElasticOptions elasticOptions
    )
    {
        this.elasticsearchClient = elasticsearchClient;
        this.elasticOptions = elasticOptions;
    }

    public async Task RunAsync()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var elasticIndicies = assemblies
            .SelectMany(x => x.GetTypes())
            .Select(t =>
                t.GetCustomAttribute(typeof(ElasticStorageElementAttribute)) is ElasticStorageElementAttribute e
                    ? e.Name
                    : null
            )
            .Where(x => x is not null)
            .Select(x => ElasticIndicesTools.CreateIndexName(x!, elasticOptions.ApplicationName, elasticOptions.Environment))
            .ToArray();

        foreach (var index in elasticIndicies)
        {
            var createIndexResponse = await elasticsearchClient.Indices.CreateAsync(index);
            if (!createIndexResponse.IsSuccess())
            {
                await Console.Error.WriteLineAsync($"Index {index} was not created");
                continue;
            }

            Console.WriteLine($"Created index {index}");

            var putMappingResponse = await elasticsearchClient.Indices.PutMappingAsync(index, descriptor => descriptor.Dynamic(DynamicMapping.True));
            if (!putMappingResponse.IsSuccess())
            {
                await Console.Error.WriteLineAsync($"Mapping in index {index} was not created ");
            }

            Console.WriteLine($"Created mapping in index {index}");
        }
    }

    private readonly ElasticsearchClient elasticsearchClient;
    private readonly ElasticOptions elasticOptions;

    public string Name => nameof(ActualizeElasticIndicesTool);
}