using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;

namespace TelemetryApp.Core.ApiTelemetry.Repository;

public interface IApiTelemetryRepository
{
    Task CreateAsync(ApiTelemetryDto apiTelemetryDto);
    Task<ApiTelemetryDto[]> FindAsync(ApiRequestInfoFilterDto filter);
}