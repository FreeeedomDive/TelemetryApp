using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;
using TelemetryApp.Api.Dto.Logs.Filter;
using TelemetryApp.Core.ApiTelemetry.Repository.Elastic;
using TelemetryApp.Core.Logs.Repository.Elastic;

namespace TelemetryApp.Core.Extensions;

public static class SearchRequestDescriptorExtensions
{
    public static SearchRequestDescriptor<ElasticLogStorageElement> GetQueries(
        this SearchRequestDescriptor<ElasticLogStorageElement> descriptor, LogFilterDto filter
    )
    {
        var filterList = new List<Query>();
        if (!string.IsNullOrEmpty(filter.Project))
        {
            filterList.Add(
                new TermQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.Project.Suffix("keyword")))
                {
                    Value = filter.Project,
                }
            );
        }

        if (!string.IsNullOrEmpty(filter.Service))
        {
            filterList.Add(
                new TermQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.Service.Suffix("keyword")))
                {
                    Value = filter.Service,
                }
            );
        }

        if (!string.IsNullOrEmpty(filter.LogLevel))
        {
            filterList.Add(
                new TermQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.LogLevel.Suffix("keyword")))
                {
                    Value = filter.LogLevel,
                }
            );
        }

        if (!string.IsNullOrEmpty(filter.Template))
        {
            filterList.Add(
                new MatchQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.Template))
                {
                    Query = filter.Template,
                    Operator = Operator.And,
                }
            );
        }

        if (filter.DateTimeRange?.From != null)
        {
            filterList.Add(
                new DateRangeQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.DateTime))
                {
                    From = filter.DateTimeRange.From,
                }
            );
        }

        if (filter.DateTimeRange?.To != null)
        {
            filterList.Add(
                new DateRangeQuery((Expression<Func<ElasticLogStorageElement, object>>)(entity => entity.DateTime))
                {
                    To = filter.DateTimeRange.To,
                }
            );
        }

        return descriptor.Query(
            queryDescriptor => queryDescriptor
                .Bool(
                    boolQueryDescriptor => boolQueryDescriptor
                        .Filter(filterList)
                )
        );
    }

    public static SearchRequestDescriptor<ElasticApiTelemetryStorageElement> GetQueries(
        this SearchRequestDescriptor<ElasticApiTelemetryStorageElement> descriptor, ApiRequestInfoFilterDto filter
    )
    {
        var filterList = new List<Query>();

        if (!string.IsNullOrEmpty(filter.Project))
        {
            filterList.Add(
                new TermQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.Project.Suffix("keyword")))
                {
                    Value = filter.Project,
                }
            );
        }

        if (!string.IsNullOrEmpty(filter.Service))
        {
            filterList.Add(
                new TermQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.Service.Suffix("keyword")))
                {
                    Value = filter.Service,
                }
            );
        }

        if (!string.IsNullOrEmpty(filter.Method))
        {
            filterList.Add(
                new TermQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.Method.Suffix("keyword")))
                {
                    Value = filter.Method,
                }
            );
        }

        if (!string.IsNullOrEmpty(filter.Route))
        {
            filterList.Add(
                new MatchQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.RoutePattern))
                {
                    Query = filter.Route,
                    Operator = Operator.And,
                }
            );
        }

        if (filter.StatusCode != null)
        {
            filterList.Add(
                new TermQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.StatusCode))
                {
                    Value = filter.StatusCode.Value,
                }
            );
        }

        if (filter.ExecutionTimeRange?.From != null)
        {
            filterList.Add(
                new NumberRangeQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.ExecutionTime))
                {
                    From = filter.ExecutionTimeRange.From,
                }
            );
        }

        if (filter.ExecutionTimeRange?.To != null)
        {
            filterList.Add(
                new NumberRangeQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.ExecutionTime))
                {
                    To = filter.ExecutionTimeRange.To,
                }
            );
        }

        if (filter.DateTimeRange?.From != null)
        {
            filterList.Add(
                new DateRangeQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.DateTime))
                {
                    From = filter.DateTimeRange.From,
                }
            );
        }

        if (filter.DateTimeRange?.To != null)
        {
            filterList.Add(
                new DateRangeQuery((Expression<Func<ElasticApiTelemetryStorageElement, object>>)(entity => entity.DateTime))
                {
                    To = filter.DateTimeRange.To,
                }
            );
        }

        return descriptor.Query(
            queryDescriptor => queryDescriptor
                .Bool(
                    boolQueryDescriptor => boolQueryDescriptor
                        .Filter(filterList)
                )
        );
    }
}