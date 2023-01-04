using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;
using TelemetryApp.Api.Dto.Logs;
using TelemetryApp.Api.Dto.Logs.Filter;

namespace TelemetryApp.Core.Logs.Repository;

public class LogRepository : ILogRepository
{
    public LogRepository(ISqlRepository<LogStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync(LogDto logDto)
    {
        await sqlRepository.CreateAsync(ToStorageElement(logDto));
    }

    public async Task<LogDto[]> FindAsync(LogFilterDto filter)
    {
        var expression = BuildExpression(filter);
        var result = await sqlRepository
            .BuildCustomQuery()
            .Where(expression)
            .OrderBy(x => x.DateTime)
            .ToArrayAsync();

        return result.Select(ToModel).ToArray();
    }

    private static Expression<Func<LogStorageElement, bool>> BuildExpression(LogFilterDto filter)
    {
        return x =>
            (string.IsNullOrEmpty(filter.Project) || x.Project == filter.Project)
            && (string.IsNullOrEmpty(filter.Service) || x.Service == filter.Service)
            && (string.IsNullOrEmpty(filter.LogLevel) || x.LogLevel == filter.LogLevel)
            && (string.IsNullOrEmpty(filter.Template) || x.Template == filter.Template)
            && (filter.DateTimeRange == null
                || (filter.DateTimeRange.From == null || filter.DateTimeRange.From <= x.DateTime)
                || (filter.DateTimeRange.To == null || x.DateTime <= filter.DateTimeRange.To)
            );
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
            Exception = JsonConvert.DeserializeObject<Exception>(storageElement.Exception)!,
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
            Params = JsonConvert.SerializeObject(dto.Params),
            Exception = JsonConvert.SerializeObject(dto.Exception),
            DateTime = DateTime.UtcNow
        };
    }

    private readonly ISqlRepository<LogStorageElement> sqlRepository;
}