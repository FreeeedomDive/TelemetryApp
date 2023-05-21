using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;
using TelemetryApp.Core.ApiTelemetry.Repository;
using TelemetryApp.Core.ApiTelemetry.Repository.PostgreSql;
using TelemetryApp.Core.ProjectServices.Repository;

namespace TelemetryApp.Core.ApiTelemetry.Service;

public class ApiTelemetryService : IApiTelemetryService
{
    public ApiTelemetryService(
        IProjectServiceRepository projectServiceRepository,
        IApiTelemetryRepository apiTelemetryRepository
    )
    {
        this.projectServiceRepository = projectServiceRepository;
        this.apiTelemetryRepository = apiTelemetryRepository;
    }

    public async Task CreateAsync(ApiTelemetryDto apiTelemetryDto)
    {
        if (
            !await projectServiceRepository.IsProjectExistAsync(apiTelemetryDto.Project)
            || !await projectServiceRepository.IsServiceExistAsync(apiTelemetryDto.Project, apiTelemetryDto.Service)
        )
        {
            await projectServiceRepository.CreateAsync(apiTelemetryDto.Project, apiTelemetryDto.Service);
        }

        await apiTelemetryRepository.CreateAsync(apiTelemetryDto);
    }

    public async Task<ApiTelemetryDto[]> FindAsync(ApiRequestInfoFilterDto filter)
    {
        return await apiTelemetryRepository.FindAsync(filter);
    }

    private readonly IProjectServiceRepository projectServiceRepository;
    private readonly IApiTelemetryRepository apiTelemetryRepository;
}