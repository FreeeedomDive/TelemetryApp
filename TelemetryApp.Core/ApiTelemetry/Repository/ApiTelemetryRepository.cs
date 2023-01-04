using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Repository;
using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;

namespace TelemetryApp.Core.ApiTelemetry.Repository;

public class ApiTelemetryRepository : IApiTelemetryRepository
{
    public ApiTelemetryRepository(ISqlRepository<ApiTelemetryStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync(ApiTelemetryDto apiTelemetryDto)
    {
        await sqlRepository.CreateAsync(ToStorageElement(apiTelemetryDto));
    }

    public async Task<ApiTelemetryDto[]> FindAsync(ApiRequestInfoFilterDto filter)
    {
        var expression = BuildExpression(filter);
        var result = await sqlRepository
            .BuildCustomQuery()
            .Where(expression)
            .OrderBy(x => x.DateTime)
            .ToArrayAsync();

        return result.Select(ToModel).ToArray();
    }

    private static Expression<Func<ApiTelemetryStorageElement, bool>> BuildExpression(ApiRequestInfoFilterDto filter)
    {
        return x =>
            (string.IsNullOrEmpty(filter.Project) || x.Project == filter.Project)
            && (string.IsNullOrEmpty(filter.Service) || x.Service == filter.Service)
            && (string.IsNullOrEmpty(filter.Method) || x.Method == filter.Method)
            && (string.IsNullOrEmpty(filter.Route) || x.Route == filter.Route)
            && (filter.StatusCode == null || x.StatusCode == filter.StatusCode)
            && (filter.ExecutionTimeRange == null
                || (
                    (filter.ExecutionTimeRange.From == null || filter.ExecutionTimeRange.From <= x.ExecutionTime)
                    && (filter.ExecutionTimeRange.To == null || x.ExecutionTime <= filter.ExecutionTimeRange.To)
                )
            )
            && (filter.DateTimeRange == null
                || (
                    (filter.DateTimeRange.From == null || filter.DateTimeRange.From <= x.DateTime)
                    && (filter.DateTimeRange.To == null || x.DateTime <= filter.DateTimeRange.To)
                )
            );
    }

    private static ApiTelemetryDto ToModel(ApiTelemetryStorageElement storageElement)
    {
        return new ApiTelemetryDto
        {
            Project = storageElement.Project,
            Service = storageElement.Service,
            Method = storageElement.Method,
            Route = storageElement.Route,
            StatusCode = storageElement.StatusCode,
            ExecutionTime = storageElement.ExecutionTime,
            DateTime = storageElement.DateTime
        };
    }

    private static ApiTelemetryStorageElement ToStorageElement(ApiTelemetryDto dto)
    {
        return new ApiTelemetryStorageElement
        {
            Id = Guid.NewGuid(),
            Project = dto.Project,
            Service = dto.Service,
            Method = dto.Method,
            Route = dto.Route,
            StatusCode = dto.StatusCode,
            ExecutionTime = dto.ExecutionTime,
            DateTime = DateTime.UtcNow
        };
    }

    private readonly ISqlRepository<ApiTelemetryStorageElement> sqlRepository;
}