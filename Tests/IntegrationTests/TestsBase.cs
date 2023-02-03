using Common.Mocks;
using NUnit.Framework;
using SqlRepositoryBase.Core.Repository;
using TelemetryApp.Core.ApiTelemetry.Repository;
using TelemetryApp.Core.ApiTelemetry.Service;
using TelemetryApp.Core.Database;
using TelemetryApp.Core.Logs.Repository;
using TelemetryApp.Core.Logs.Service;
using TelemetryApp.Core.ProjectServices.Repository;

namespace IntegrationTests;

public class TestsBase
{
    [SetUp]
    public void GlobalSetUp()
    {
        var databaseContext = new DatabaseContext();

        var projectsSqlRepository = new SqlRepository<ProjectServiceStorageElement>(databaseContext);
        ProjectServiceRepository = new ProjectServiceRepository(projectsSqlRepository);

        var logsSqlRepository = new SqlRepository<LogStorageElement>(databaseContext);
        var logsRepository = new LogRepository(logsSqlRepository);

        var apiSqlRepository = new SqlRepository<ApiTelemetryStorageElement>(databaseContext);
        var apiRepository = new ApiTelemetryRepository(apiSqlRepository);

        LogService = new LogService(ProjectServiceRepository, logsRepository, new ExceptionEventsSentryServiceMock());
        ApiTelemetryService = new ApiTelemetryService(ProjectServiceRepository, apiRepository);

        ProjectName = Guid.NewGuid().ToString();
        ServiceName = Guid.NewGuid().ToString();
    }

    protected ProjectServiceRepository ProjectServiceRepository { get; private set; } = null!;
    protected ILogService LogService { get; private set; } = null!;
    protected IApiTelemetryService ApiTelemetryService { get; private set; } = null!;

    protected string ProjectName { get; private set; } = null!;
    protected string ServiceName { get; private set; } = null!;

    protected Random Random { get; } = new();
}