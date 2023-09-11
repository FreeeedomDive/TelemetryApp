using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;
using TelemetryApp.Core.ApiTelemetry.Repository.Elastic;
using TelemetryApp.Core.ApiTelemetry.Repository.PostgreSql;

namespace TelemetryApp.Core.ApiTelemetry.Repository;

public class CompositeApiTelemetryRepository : IApiTelemetryRepository
{
    public CompositeApiTelemetryRepository(
        SqlApiTelemetryRepository sqlApiTelemetryRepository,
        ElasticApiTelemetryRepository elasticApiTelemetryRepository
    )
    {
        this.sqlApiTelemetryRepository = sqlApiTelemetryRepository;
        this.elasticApiTelemetryRepository = elasticApiTelemetryRepository;
    }

    public Task CreateAsync(ApiTelemetryDto apiTelemetryDto)
    {
        return Task.WhenAll(
            sqlApiTelemetryRepository.CreateAsync(apiTelemetryDto),
            elasticApiTelemetryRepository.CreateAsync(apiTelemetryDto)
        );
    }

    public Task<ApiTelemetryDto[]> FindAsync(ApiRequestInfoFilterDto filter)
    {
        return sqlApiTelemetryRepository.FindAsync(filter);
    }

    private readonly ElasticApiTelemetryRepository elasticApiTelemetryRepository;

    private readonly SqlApiTelemetryRepository sqlApiTelemetryRepository;
}