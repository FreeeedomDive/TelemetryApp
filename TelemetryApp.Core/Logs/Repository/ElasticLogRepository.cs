using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Transport;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Core.Logs.Repository;

public class ElasticLogRepository : ILogRepository
{
    public ElasticLogRepository(string uri, string username, string password, string index)
    {
        var settings = new ElasticsearchClientSettings(new Uri(uri))
            .DefaultIndex(index)
            .Authentication(new BasicAuthentication(username, password));
        elasticsearchClient = new ElasticsearchClient(settings);
    }

    public async Task CreateAsync(LogDto logDto)
    {
        var response = await elasticsearchClient.IndexAsync(logDto);

        if (!response.IsSuccess())
        {
            //куда-то нужно будет положить ошибку, на твоё усмотрение
        }
    }

    public async Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        var searchDescriptor = new SearchRequestDescriptor<LogDto>()
            .Query(descriptor => descriptor
                .Bool(queryDescriptor => queryDescriptor.Must(mustDescriptor =>
                {
                    if (!string.IsNullOrEmpty(filter.Service))
                        mustDescriptor.Term(l => l.Service, filter.Service);
                    if (!string.IsNullOrEmpty(filter.Project))
                        mustDescriptor.Term(l => l.Project, filter.Project);
                    if (!string.IsNullOrEmpty(filter.LogLevel))
                        mustDescriptor.Term(l => l.LogLevel, filter.LogLevel);
                    if (!string.IsNullOrEmpty(filter.Template))
                        mustDescriptor.Match(matchQueryDescriptor => matchQueryDescriptor
                            .Field(l => l.Template)
                            .Query(filter.Template));
                    if (filter.DateTimeRange?.From != null)
                        mustDescriptor.Range(rangeQueryDescriptor => rangeQueryDescriptor
                            .DateRange(dateRangeQueryDescriptor => dateRangeQueryDescriptor
                                .Field(l => l.DateTime)
                                .From(filter.DateTimeRange.From)));
                    if (filter.DateTimeRange?.To != null)
                        mustDescriptor.Range(rangeQueryDescriptor => rangeQueryDescriptor
                            .DateRange(dateRangeQueryDescriptor => dateRangeQueryDescriptor
                                .Field(l => l.DateTime)
                                .To(filter.DateTimeRange.To)));
                })))
            .Sort(descriptor => descriptor
                .Field(l => l.DateTime, sortDescriptor => sortDescriptor.Order(SortOrder.Desc)));
        var response = await elasticsearchClient.SearchAsync<LogDto>(searchDescriptor);

        if (!response.IsSuccess())
        {
            //куда-то нужно будет положить ошибку, на твоё усмотрение
            return Array.Empty<LogDto>();
        }

        return response.Documents.ToArray();
    }

    public async Task BulkAllObservable(IEnumerable<LogDto> logDtos)
    {
        elasticsearchClient.BulkAll(logDtos, descriptor => descriptor
                .BackOffTime("30s")
                .BackOffRetries(2)
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000))
            .Wait(TimeSpan.FromMinutes(15), response => { });
    }

    public async Task CreateIndexWithDynamicMapping(string indexName)
    {
        var response = await elasticsearchClient.Indices.CreateAsync(indexName, descriptor => descriptor
            .Mappings(mappingDescriptor => mappingDescriptor.Dynamic(DynamicMapping.True)));
        
        if (!response.IsSuccess())
        {
            //куда-то нужно будет положить ошибку, на твоё усмотрение
        }
    }

    private readonly ElasticsearchClient elasticsearchClient;
}