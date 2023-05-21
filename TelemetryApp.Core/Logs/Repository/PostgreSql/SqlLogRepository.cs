using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Core.Logs.Repository.PostgreSql;

public class SqlLogRepository : ILogRepository
{
    public SqlLogRepository(ISqlRepository<LogStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync(LogDto logDto)
    {
        await sqlRepository.CreateAsync(ToStorageElement(logDto));
    }

    public async Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        var result = await sqlRepository
            .BuildCustomQuery()
            .WhereIf(!string.IsNullOrEmpty(filter.Project), x => x.Project == filter.Project)
            .WhereIf(!string.IsNullOrEmpty(filter.Service), x => x.Service == filter.Service)
            .WhereIf(!string.IsNullOrEmpty(filter.LogLevel), x => x.LogLevel == filter.LogLevel)
            .WhereIf(!string.IsNullOrEmpty(filter.Template), x => x.Template == filter.Template)
            .WhereIf(filter.DateTimeRange?.From != null, x => filter.DateTimeRange!.From <= x.DateTime)
            .WhereIf(filter.DateTimeRange?.To != null, x => x.DateTime <= filter.DateTimeRange!.To)
            .OrderByDescending(x => x.DateTime)
            .ToArrayAsync();

        return result.Select(ToModel).ToArray();
    }

    private static LogDto ToModel(LogStorageElement storageElement)
    {
        return new LogDto
        {
            Project = storageElement.Project,
            Service = storageElement.Service,
            LogLevel = storageElement.LogLevel,
            Template = storageElement.Template,
            Params = JsonConvert.DeserializeObject<string[]>(storageElement.Params)!,
            Exception = storageElement.Exception,
            DateTime = storageElement.DateTime
        };
    }

    private static LogStorageElement ToStorageElement(LogDto dto)
    {
        return new LogStorageElement
        {
            Id = Guid.NewGuid(),
            Project = dto.Project,
            Service = dto.Service,
            LogLevel = dto.LogLevel,
            Template = dto.Template,
            Params = JsonConvert.SerializeObject(dto.Params, Formatting.Indented),
            Exception = dto.Exception,
            DateTime = dto.DateTime ?? DateTime.UtcNow
        };
    }

    private readonly ISqlRepository<LogStorageElement> sqlRepository;
}