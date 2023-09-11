using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;
using TelemetryApp.Core.Logs.Repository.Elastic;
using TelemetryApp.Core.Logs.Repository.PostgreSql;

namespace TelemetryApp.Core.Logs.Repository;

public class CompositeLogRepository : ILogRepository
{
    public CompositeLogRepository(
        SqlLogRepository sqlLogRepository,
        ElasticLogRepository elasticLogRepository
    )
    {
        this.sqlLogRepository = sqlLogRepository;
        this.elasticLogRepository = elasticLogRepository;
    }

    public Task CreateAsync(LogDto logDto)
    {
        return Task.WhenAll(
            sqlLogRepository.CreateAsync(logDto),
            elasticLogRepository.CreateAsync(logDto)
        );
    }

    public Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        return elasticLogRepository.FindAsync(filter);
    }

    private readonly ElasticLogRepository elasticLogRepository;

    private readonly SqlLogRepository sqlLogRepository;
}