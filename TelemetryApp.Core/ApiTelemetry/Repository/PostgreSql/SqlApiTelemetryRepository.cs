using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;
using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;

namespace TelemetryApp.Core.ApiTelemetry.Repository.PostgreSql;

public class SqlApiTelemetryRepository : IApiTelemetryRepository
{
    public SqlApiTelemetryRepository(ISqlRepository<ApiTelemetryStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task CreateAsync(ApiTelemetryDto apiTelemetryDto)
    {
        await sqlRepository.CreateAsync(ToStorageElement(apiTelemetryDto));
    }

    public async Task<ApiTelemetryDto[]> FindAsync(ApiRequestInfoFilterDto filter)
    {
        var result = await sqlRepository
            .BuildCustomQuery()
            .WhereIf(!string.IsNullOrEmpty(filter.Project), x => x.Project == filter.Project)
            .WhereIf(!string.IsNullOrEmpty(filter.Service), x => x.Service == filter.Service)
            .WhereIf(!string.IsNullOrEmpty(filter.Method), x => x.Method == filter.Method)
            .WhereIf(!string.IsNullOrEmpty(filter.Route), x => x.Route == filter.Route)
            .WhereIf(filter.StatusCode != null, x => x.StatusCode == filter.StatusCode)
            .WhereIf(filter.ExecutionTimeRange?.From != null, x => filter.ExecutionTimeRange!.From <= x.ExecutionTime)
            .WhereIf(filter.ExecutionTimeRange?.To != null, x => x.ExecutionTime <= filter.ExecutionTimeRange!.To)
            .WhereIf(filter.DateTimeRange?.From != null, x => filter.DateTimeRange!.From <= x.DateTime)
            .WhereIf(filter.DateTimeRange?.To != null, x => x.DateTime <= filter.DateTimeRange!.To)
            .OrderByDescending(x => x.DateTime)
            .ToArrayAsync();

        return result.Select(ToModel).ToArray();
    }

    private static ApiTelemetryDto ToModel(ApiTelemetryStorageElement storageElement)
    {
        return new ApiTelemetryDto
        {
            Project = storageElement.Project,
            Service = storageElement.Service,
            Method = storageElement.Method,
            RoutePattern = storageElement.Route,
            RouteParametersValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(storageElement.RouteValues)!,
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
            Route = dto.RoutePattern,
            RouteValues = JsonConvert.SerializeObject(dto.RouteParametersValues, Formatting.Indented),
            StatusCode = dto.StatusCode,
            ExecutionTime = dto.ExecutionTime,
            DateTime = dto.DateTime ?? DateTime.UtcNow
        };
    }

    private readonly ISqlRepository<ApiTelemetryStorageElement> sqlRepository;
}