using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;

namespace TelemetryApp.Core.ApiTelemetry.Service;

public interface IApiTelemetryService
{
    Task CreateAsync(ApiTelemetryDto apiTelemetryDto);
    Task<ApiTelemetryDto[]> FindAsync(ApiRequestInfoFilterDto filter);
}