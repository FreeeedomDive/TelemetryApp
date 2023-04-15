using System.Reflection;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TelemetryApp.Core.Database;
using TelemetryApp.Core.Elastic;
using TelemetryApp.Core.Logs.Repository.Elastic;

namespace TelemetryApp.Tools.Tools.ElasticActualizer;

public class MigrateLogsFromPostgreSqlToElastic : ITool
{
    public MigrateLogsFromPostgreSqlToElastic(
        ElasticsearchClient elasticsearchClient,
        ElasticOptions elasticOptions,
        DatabaseContext databaseContext
    )
    {
        this.elasticsearchClient = elasticsearchClient;
        this.elasticOptions = elasticOptions;
        this.databaseContext = databaseContext;
    }

    public async Task RunAsync()
    {
        var oldLogs = await databaseContext.LogsStorage.ToArrayAsync();

        var newLogs = oldLogs.Select(x => new ElasticLogStorageElement
        {
            Id = x.Id,
            Project = x.Project,
            Service = x.Service,
            LogLevel = x.LogLevel,
            Template = x.Template,
            Params = JsonConvert.DeserializeObject<string[]>(x.Params)!,
            Exception = x.Exception,
            DateTime = x.DateTime,
        }).ToArray();

        Console.WriteLine($"Converting {newLogs.Length} elements");
        var attribute = (typeof(ElasticLogStorageElement).GetCustomAttribute(typeof(ElasticStorageElementAttribute))
            as ElasticStorageElementAttribute)!;
        var index = ElasticIndicesTools.CreateIndexName(attribute.Name, elasticOptions.ApplicationName, elasticOptions.Environment);
        elasticsearchClient.BulkAll(newLogs, descriptor => descriptor
                .Index(index)
                .BackOffTime("30s")
                .BackOffRetries(2)
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000))
            .Wait(TimeSpan.FromMinutes(15), _ => { });
    }

    public string Name => nameof(MigrateLogsFromPostgreSqlToElastic);

    private readonly ElasticsearchClient elasticsearchClient;
    private readonly ElasticOptions elasticOptions;
    private readonly DatabaseContext databaseContext;
}