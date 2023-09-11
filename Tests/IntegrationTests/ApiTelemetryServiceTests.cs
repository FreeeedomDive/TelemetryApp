using Common.Extensions;
using Common.Generators;
using FluentAssertions;
using NUnit.Framework;
using TelemetryApp.Api.Dto;
using TelemetryApp.Api.Dto.ApiTelemetry;
using TelemetryApp.Api.Dto.ApiTelemetry.Filter;

namespace IntegrationTests;

public class ApiTelemetryServiceTests : TestsBase
{
    [Test]
    public async Task CreateAndFind()
    {
        var currentProjectApiLogs = await ApiTelemetryService.FindAsync(
            new ApiRequestInfoFilterDto
            {
                Project = ProjectName,
                Service = ServiceName,
            }
        );

        var id = Guid.NewGuid().ToString();
        currentProjectApiLogs.Should().BeEmpty();
        await ApiTelemetryService.CreateAsync(
            new ApiTelemetryDto
            {
                Project = ProjectName,
                Service = ServiceName,
                Method = ApiGenerator.GenerateApiMethod(),
                RoutePattern = "api/telemetryTests/{id}/createAndFind",
                RouteParametersValues = new Dictionary<string, string>
                {
                    { "id", id },
                },
                ExecutionTime = Random.NextInt64(2000),
                StatusCode = ApiGenerator.GenerateStatusCode(),
            }
        );

        currentProjectApiLogs = await ApiTelemetryService.FindAsync(
            new ApiRequestInfoFilterDto
            {
                Project = ProjectName,
                Service = ServiceName,
            }
        );

        currentProjectApiLogs.Length.Should().Be(1);
    }

    [Test]
    public async Task FindByProjectAndService()
    {
        var projects = new[] { ProjectName, Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        var services = new[] { ServiceName, Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        const int recordsToWrite = 50;
        var expected = 0;

        for (var i = 0; i < recordsToWrite; i++)
        {
            var project = projects.SelectRandom();
            var service = services.SelectRandom();
            expected += project == ProjectName && service == ServiceName ? 1 : 0;
            await ApiTelemetryService.CreateAsync(
                new ApiTelemetryDto
                {
                    Project = project,
                    Service = service,
                    Method = ApiGenerator.GenerateApiMethod(),
                    RoutePattern = "api/telemetryTests/{id}/findByProjectAndService",
                    RouteParametersValues = new Dictionary<string, string>
                    {
                        { "id", Guid.NewGuid().ToString() },
                    },
                    ExecutionTime = Random.NextInt64(2000),
                    StatusCode = ApiGenerator.GenerateStatusCode(),
                }
            );
        }

        var currentProjectApiLogs = await ApiTelemetryService.FindAsync(
            new ApiRequestInfoFilterDto
            {
                Project = ProjectName,
                Service = ServiceName,
            }
        );

        currentProjectApiLogs.Length.Should().Be(expected);
    }

    [Test]
    public async Task FindByMethod()
    {
        const int recordsToWrite = 50;
        var methodsCounter = ApiGenerator.Methods.ToDictionary(x => x, _ => 0);

        for (var i = 0; i < recordsToWrite; i++)
        {
            var method = ApiGenerator.GenerateApiMethod();
            methodsCounter[method]++;
            await ApiTelemetryService.CreateAsync(
                new ApiTelemetryDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    Method = method,
                    RoutePattern = "api/telemetryTests/{id}/findByMethod",
                    RouteParametersValues = new Dictionary<string, string>
                    {
                        { "id", Guid.NewGuid().ToString() },
                    },
                    ExecutionTime = Random.NextInt64(2000),
                    StatusCode = ApiGenerator.GenerateStatusCode(),
                }
            );
        }

        foreach (var method in ApiGenerator.Methods)
        {
            var logsByMethod = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    Method = method,
                }
            );

            logsByMethod.Length.Should().Be(methodsCounter[method]);
        }
    }

    [Test]
    public async Task FindByStatusCode()
    {
        const int recordsToWrite = 50;
        var statusCodesCounter = ApiGenerator.StatusCodes.ToDictionary(x => x, _ => 0);

        for (var i = 0; i < recordsToWrite; i++)
        {
            var statusCode = ApiGenerator.GenerateStatusCode();
            statusCodesCounter[statusCode]++;
            await ApiTelemetryService.CreateAsync(
                new ApiTelemetryDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    Method = ApiGenerator.GenerateApiMethod(),
                    RoutePattern = "api/telemetryTests/{id}/findByStatusCode",
                    RouteParametersValues = new Dictionary<string, string>
                    {
                        { "id", Guid.NewGuid().ToString() },
                    },
                    ExecutionTime = Random.NextInt64(2000),
                    StatusCode = statusCode,
                }
            );
        }

        foreach (var statusCode in ApiGenerator.StatusCodes)
        {
            var statusCodeLogs = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    StatusCode = statusCode,
                }
            );

            statusCodeLogs.Length.Should().Be(statusCodesCounter[statusCode]);
        }
    }

    [Test]
    public async Task FindByExecutionTime_StartRange()
    {
        const int itemsCount = 20;
        const int step = 50;
        await WriteLogsWithExecutionTimesAsync(Enumerable.Range(1, itemsCount).Select(x => x * step));

        ApiTelemetryDto[] logs;
        var from = step / 2;
        var expectedCurrentLogsCount = itemsCount;
        do
        {
            logs = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    ExecutionTimeRange = new ExecutionTimeRange
                    {
                        From = from,
                    },
                }
            );
            logs.Length.Should().Be(expectedCurrentLogsCount--);
            from += step;
        }
        while (logs.Length > 0);
    }

    [Test]
    public async Task FindByExecutionTime_EndRange()
    {
        const int itemsCount = 20;
        const int step = 50;
        await WriteLogsWithExecutionTimesAsync(Enumerable.Range(1, itemsCount).Select(x => x * step));

        var logs = Array.Empty<ApiTelemetryDto>();
        var to = step / 2;
        var expectedCurrentLogsCount = 0;
        while (logs.Length < itemsCount)
        {
            logs = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    ExecutionTimeRange = new ExecutionTimeRange
                    {
                        To = to,
                    },
                }
            );
            logs.Length.Should().Be(expectedCurrentLogsCount++);
            to += step;
        }
    }

    [Test]
    public async Task FindByExecutionTime_FullRange()
    {
        const int itemsCount = 20;
        var executionTimes = Enumerable.Range(1, itemsCount).Select(_ => Random.Next(1500)).ToArray();
        var max = executionTimes.Max();
        await WriteLogsWithExecutionTimesAsync(executionTimes);

        for (var i = 0; i < 20; i++)
        {
            var from = Random.Next(0, max - 1);
            var to = Random.Next(from, max);
            var expected = executionTimes.Count(x => from <= x && x <= to);
            var actual = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    ExecutionTimeRange = new ExecutionTimeRange
                    {
                        From = from,
                        To = to,
                    },
                }
            );
            actual.Length.Should().Be(expected);
        }
    }

    [Test]
    public async Task FindByDateTime_StartRange()
    {
        var startDate = DateTime.UtcNow.Date;
        const int itemsCount = 20;
        const int stepInMinutes = 30;
        await WriteLogsWithDateTimesAsync(Enumerable.Range(1, itemsCount).Select(x => startDate.AddMinutes(x * stepInMinutes)));

        ApiTelemetryDto[] logs;
        // ReSharper disable once PossibleLossOfFraction
        var from = startDate.AddMinutes(stepInMinutes / 2);
        var expectedCurrentLogsCount = itemsCount;
        do
        {
            logs = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    DateTimeRange = new DateTimeRange
                    {
                        From = from,
                    },
                }
            );
            logs.Length.Should().Be(expectedCurrentLogsCount--);
            from = from.AddMinutes(stepInMinutes);
        }
        while (logs.Length > 0);
    }

    [Test]
    public async Task FindByDateTime_EndRange()
    {
        var startDate = DateTime.UtcNow.Date;
        const int itemsCount = 20;
        const int stepInMinutes = 30;
        await WriteLogsWithDateTimesAsync(Enumerable.Range(1, itemsCount).Select(x => startDate.AddMinutes(x * stepInMinutes)));

        var logs = Array.Empty<ApiTelemetryDto>();
        // ReSharper disable once PossibleLossOfFraction
        var to = startDate.AddMinutes(stepInMinutes / 2);
        var expectedCurrentLogsCount = 0;
        while (logs.Length < itemsCount)
        {
            logs = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    DateTimeRange = new DateTimeRange
                    {
                        To = to,
                    },
                }
            );
            logs.Length.Should().Be(expectedCurrentLogsCount++);
            to = to.AddMinutes(stepInMinutes);
        }
    }

    [Test]
    public async Task FindByDateTime_FullRange()
    {
        var startDate = DateTime.UtcNow.Date;
        const int itemsCount = 20;
        const int maxMinutes = 1000;
        var dates = Enumerable.Range(1, itemsCount).Select(_ => startDate.AddMinutes(Random.Next(maxMinutes))).ToArray();
        await WriteLogsWithDateTimesAsync(dates);

        for (var i = 0; i < 20; i++)
        {
            var from = Random.Next(0, maxMinutes - 1);
            var fromDate = startDate.AddMinutes(from);
            var toDate = startDate.AddMinutes(Random.Next(from, maxMinutes));
            var expected = dates.Count(x => fromDate <= x && x <= toDate);
            var actual = await ApiTelemetryService.FindAsync(
                new ApiRequestInfoFilterDto
                {
                    Project = ProjectName,
                    Service = ServiceName,
                    DateTimeRange = new DateTimeRange
                    {
                        From = fromDate,
                        To = toDate,
                    },
                }
            );
            actual.Length.Should().Be(expected);
        }
    }

    private async Task WriteLogsWithExecutionTimesAsync(IEnumerable<int> executionTimes)
    {
        var dtos = executionTimes
                   .Select(
                       executionTime => new ApiTelemetryDto
                       {
                           Project = ProjectName,
                           Service = ServiceName,
                           Method = ApiGenerator.GenerateApiMethod(),
                           RoutePattern = "api/telemetryTests/{id}/findByExecutionTime",
                           RouteParametersValues = new Dictionary<string, string>
                           {
                               { "id", Guid.NewGuid().ToString() },
                           },
                           ExecutionTime = executionTime,
                           StatusCode = 200,
                       }
                   )
                   .ToArray();

        foreach (var dto in dtos)
        {
            await ApiTelemetryService.CreateAsync(dto);
        }
    }

    private async Task WriteLogsWithDateTimesAsync(IEnumerable<DateTime> dateTimes)
    {
        var dtos = dateTimes
                   .Select(
                       dateTime => new ApiTelemetryDto
                       {
                           Project = ProjectName,
                           Service = ServiceName,
                           Method = ApiGenerator.GenerateApiMethod(),
                           RoutePattern = "api/telemetryTests/{id}/findByDateTime",
                           RouteParametersValues = new Dictionary<string, string>
                           {
                               { "id", Guid.NewGuid().ToString() },
                           },
                           ExecutionTime = Random.Next(2000),
                           StatusCode = 200,
                           DateTime = dateTime,
                       }
                   )
                   .ToArray();

        foreach (var dto in dtos)
        {
            await ApiTelemetryService.CreateAsync(dto);
        }
    }
}