using FluentAssertions;
using NUnit.Framework;
using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.Logs;

namespace IntegrationTests;

public class ProjectsTests : TestsBase
{
    [Test]
    public async Task AddLog_Should_AddProjectAndService()
    {
        await LogService.CreateAsync(new LogDto
        {
            Project = ProjectName,
            Service = ServiceName,
            LogLevel = "INFO",
            Template = "Test log with {param}",
            Params = new[] { "test param" },
            Exception = "",
        });

        (await ProjectServiceRepository.IsProjectExistAsync(ProjectName)).Should().BeTrue();
        (await ProjectServiceRepository.IsServiceExistAsync(ProjectName, ServiceName)).Should().BeTrue();
    }

    [Test]
    public async Task AddApiLog_Should_AddProjectAndService()
    {
        await ApiTelemetryService.CreateAsync(new ApiTelemetryDto
        {
            Project = ProjectName,
            Service = ServiceName,
            Method = "GET",
            RoutePattern = "/api/test/{param}",
            RouteParametersValues = new Dictionary<string, string>()
            {
                { "param", "69" }
            },
            StatusCode = 200,
            ExecutionTime = 228,
        });

        (await ProjectServiceRepository.IsProjectExistAsync(ProjectName)).Should().BeTrue();
        (await ProjectServiceRepository.IsServiceExistAsync(ProjectName, ServiceName)).Should().BeTrue();
    }
}