using Common.ClientBuilder;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;

namespace ApiClientTests;

public class ClientsTests
{
    [SetUp]
    public void Setup()
    {
        project = Guid.NewGuid().ToString();
        service = Guid.NewGuid().ToString();
        apiTelemetryClient = ApiTelemetryClientBuilder.Build(project, service);
        loggerClient = LoggerClientBuilder.Build(project, service);
    }

    [Test]
    public async Task TestApiTelemetryClient()
    {
        await apiTelemetryClient.CreateAsync("GET", "/", new Dictionary<string, string>(), 200, 228);
    }

    [Test]
    public async Task TestLoggerDebug()
    {
        await loggerClient.DebugAsync("Test {a} {b} {c}", 1, 2, 3);
    }

    [Test]
    public async Task TestLoggerInfo()
    {
        await loggerClient.InfoAsync("Test {a} {b} {c}", 1, 2, 3);
    }

    [Test]
    public async Task TestLoggerWarn()
    {
        await loggerClient.WarnAsync("Test {a} {b} {c}", 1, 2, 3);
    }

    [Test]
    public async Task TestLoggerWarnWithException()
    {
        await loggerClient.WarnAsync(new Exception(Guid.NewGuid().ToString()), "Test {a} {b} {c}", 1, 2, 3);
    }

    [Test]
    public async Task TestLoggerError()
    {
        await loggerClient.ErrorAsync("Test {a} {b} {c}", 1, 2, 3);
    }

    [Test]
    public async Task TestLoggerErrorWithException()
    {
        await loggerClient.ErrorAsync(new Exception(Guid.NewGuid().ToString()), "Test {a} {b} {c}", 1, 2, 3);
    }

    private IApiTelemetryClient apiTelemetryClient;
    private ILoggerClient loggerClient;

    private string project = "";
    private string service = "";
}