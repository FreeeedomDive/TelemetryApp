using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Core.Logs.Repository.Elastic;

public static class SearchRequestDescriptorExtensions
{
    public static SearchRequestDescriptor<ElasticLogStorageElement> GetQueries(
        this SearchRequestDescriptor<ElasticLogStorageElement> descriptor, LogFilterDto filter)
    {
        var filterList = new List<Query>();
        if (!string.IsNullOrEmpty(filter.Project))
            filterList.Add(new TermQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.Project.Suffix("keyword")))
            {
                Value = filter.Project
            });
        if (!string.IsNullOrEmpty(filter.Service))
            filterList.Add(new TermQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.Service.Suffix("keyword")))
            {
                Value = filter.Service
            });
        if (!string.IsNullOrEmpty(filter.LogLevel))
            filterList.Add(new TermQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.LogLevel.Suffix("keyword")))
            {
                Value = filter.LogLevel
            });
        if (!string.IsNullOrEmpty(filter.Template))
            filterList.Add(new MatchQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.Template))
            {
                Query = filter.Template
            });
        if (filter.DateTimeRange?.From != null)
            filterList.Add(new DateRangeQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.DateTime))
            {
                From = filter.DateTimeRange.From
            });
        if (filter.DateTimeRange?.To != null)
            filterList.Add(new DateRangeQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.DateTime))
            {
                To = filter.DateTimeRange.To
            });
        return descriptor.Query(queryDescriptor => queryDescriptor
            .Bool(boolQueryDescriptor => boolQueryDescriptor
                .Filter(filterList)));
    }
}