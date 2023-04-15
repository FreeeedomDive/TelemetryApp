using System.Reflection;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;
using TelemetryApp.Core.Elastic;
using TelemetryApp.Core.Extensions;
using TelemetryApp.Core.Logs.Repository.Elastic;

namespace TelemetryApp.Core.ApiTelemetry.Repository.Elastic;

public class ElasticApiTelemetryRepository : IApiTelemetryRepository
{
    public ElasticApiTelemetryRepository(
        ElasticsearchClient elasticsearchClient,
        IOptions<ElasticOptions> elasticOptions
    )
    {
        this.elasticsearchClient = elasticsearchClient;
        var attribute = typeof(ElasticApiTelemetryStorageElement).GetCustomAttribute(typeof(ElasticStorageElementAttribute));
        if (attribute is not ElasticStorageElementAttribute indexAttribute)
        {
            throw new InvalidOperationException($"{nameof(ElasticApiTelemetryStorageElement)} is not an Elastic storage element");
        }

        var options = elasticOptions.Value;
        index = ElasticIndicesTools.CreateIndexName(indexAttribute.Name, options.ApplicationName, options.Environment);
    }

    public async Task CreateAsync(ApiTelemetryDto apiTelemetryDto)
    {
        var response = await elasticsearchClient.IndexAsync(ToStorageElement(apiTelemetryDto), index);
        if (!response.IsSuccess())
        {
            throw new ElasticInsertValueException(response.ApiCallDetails.OriginalException.Message);
        }
    }

    public async Task<ApiTelemetryDto[]> FindAsync(ApiRequestInfoFilterDto filter)
    {
        var searchDescriptor = new SearchRequestDescriptor<ElasticApiTelemetryStorageElement>()
            .Index(index)
            .Size(10000)
            .GetQueries(filter)
            .Sort(descriptor => descriptor.Field(l => l.DateTime, sortDescriptor => sortDescriptor.Order(SortOrder.Desc)));
        var response = await elasticsearchClient.SearchAsync(searchDescriptor);

        return response.IsSuccess()
            ? response.Documents.Select(ToModel).ToArray()
            : throw new ElasticReadValuesException(response.ApiCallDetails.OriginalException.Message);
    }

    private static ApiTelemetryDto ToModel(ElasticApiTelemetryStorageElement storageElement)
    {
        return new ApiTelemetryDto
        {
            Project = storageElement.Project,
            Service = storageElement.Service,
            Method = storageElement.Method,
            RoutePattern = storageElement.RoutePattern,
            RouteParametersValues = storageElement.RouteParametersValues,
            StatusCode = storageElement.StatusCode,
            ExecutionTime = storageElement.ExecutionTime,
            DateTime = storageElement.DateTime
        };
    }

    private static ElasticApiTelemetryStorageElement ToStorageElement(ApiTelemetryDto dto)
    {
        return new ElasticApiTelemetryStorageElement
        {
            Id = Guid.NewGuid(),
            Project = dto.Project,
            Service = dto.Service,
            Method = dto.Method,
            RoutePattern = dto.RoutePattern,
            RouteParametersValues = dto.RouteParametersValues,
            StatusCode = dto.StatusCode,
            ExecutionTime = dto.ExecutionTime,
            DateTime = dto.DateTime ?? DateTime.UtcNow
        };
    }

    private readonly string index;
    private readonly ElasticsearchClient elasticsearchClient;
}