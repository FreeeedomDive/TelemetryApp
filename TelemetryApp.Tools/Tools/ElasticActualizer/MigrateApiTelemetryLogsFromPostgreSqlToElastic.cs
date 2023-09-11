using System.Reflection;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TelemetryApp.Core.ApiTelemetry.Repository.Elastic;
using TelemetryApp.Core.Database;
using TelemetryApp.Core.Elastic;

namespace TelemetryApp.Tools.Tools.ElasticActualizer;

public class MigrateApiTelemetryLogsFromPostgreSqlToElastic : ITool
{
    public MigrateApiTelemetryLogsFromPostgreSqlToElastic(
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
        var oldLogs = await databaseContext.ApiTelemetryStorage.ToArrayAsync();

        var newLogs = oldLogs.Select(
            x => new ElasticApiTelemetryStorageElement
            {
                Id = x.Id,
                Project = x.Project,
                Service = x.Service,
                Method = x.Method,
                StatusCode = x.StatusCode,
                RoutePattern = x.Route,
                RouteParametersValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(x.RouteValues)!,
                ExecutionTime = x.ExecutionTime,
                DateTime = x.DateTime,
            }
        ).ToArray();

        Console.WriteLine($"Converting {newLogs.Length} elements");
        var attribute = (typeof(ElasticApiTelemetryStorageElement).GetCustomAttribute(typeof(ElasticStorageElementAttribute))
            as ElasticStorageElementAttribute)!;
        var index = ElasticIndicesTools.CreateIndexName(attribute.Name, elasticOptions.ApplicationName, elasticOptions.Environment);
        elasticsearchClient.BulkAll(
                               newLogs, descriptor => descriptor
                                                      .Index(index)
                                                      .BackOffTime("30s")
                                                      .BackOffRetries(2)
                                                      .RefreshOnCompleted()
                                                      .MaxDegreeOfParallelism(Environment.ProcessorCount)
                                                      .Size(1000)
                           )
                           .Wait(TimeSpan.FromMinutes(15), _ => { });
    }

    public string Name => nameof(MigrateApiTelemetryLogsFromPostgreSqlToElastic);
    private readonly DatabaseContext databaseContext;
    private readonly ElasticOptions elasticOptions;
    private readonly ElasticsearchClient elasticsearchClient;
}