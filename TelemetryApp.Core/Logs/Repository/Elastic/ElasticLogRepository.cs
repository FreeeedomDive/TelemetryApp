using System.Reflection;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;
using TelemetryApp.Core.Elastic;

namespace TelemetryApp.Core.Logs.Repository.Elastic;

public class ElasticLogRepository : ILogRepository
{
    public ElasticLogRepository(ElasticsearchClient elasticsearchClient, IOptions<ElasticOptions> elasticOptions)
    {
        this.elasticsearchClient = elasticsearchClient;
        var attribute = typeof(ElasticLogStorageElement).GetCustomAttribute(typeof(ElasticStorageElementAttribute));
        if (attribute is not ElasticStorageElementAttribute indexAttribute)
        {
            throw new InvalidOperationException($"{nameof(ElasticLogStorageElement)} is not an Elastic storage element");
        }

        var options = elasticOptions.Value;
        index = ElasticIndicesTools.CreateIndexName(indexAttribute.Name, options.ApplicationName, options.Environment);
    }

    public async Task CreateAsync(LogDto logDto)
    {
        await elasticsearchClient.IndexAsync(ToStorageElement(logDto), index);
    }

    public async Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        var searchDescriptor = new SearchRequestDescriptor<ElasticLogStorageElement>()
            .Index(index)
            .Size(10000)
            .GetQueries(filter)
            .Sort(descriptor => descriptor
                .Field(l => l.DateTime, sortDescriptor => sortDescriptor.Order(SortOrder.Desc)));
        var response = await elasticsearchClient.SearchAsync(searchDescriptor);

        return response.IsSuccess() ?
            response.Documents.Select(ToModel).ToArray() :
            Array.Empty<LogDto>();
    }

    private static LogDto ToModel(ElasticLogStorageElement storageElement)
    {
        return new LogDto
        {
            Project = storageElement.Project,
            Service = storageElement.Service,
            LogLevel = storageElement.LogLevel,
            Template = storageElement.Template,
            Params = storageElement.Params,
            Exception = storageElement.Exception,
            DateTime = storageElement.DateTime
        };
    }

    private static ElasticLogStorageElement ToStorageElement(LogDto dto)
    {
        return new ElasticLogStorageElement
        {
            Id = Guid.NewGuid(),
            Project = dto.Project,
            Service = dto.Service,
            LogLevel = dto.LogLevel,
            Template = dto.Template,
            Params = dto.Params,
            Exception = dto.Exception,
            DateTime = dto.DateTime ?? DateTime.UtcNow
        };
    }

    private readonly string index;
    private readonly ElasticsearchClient elasticsearchClient;
}