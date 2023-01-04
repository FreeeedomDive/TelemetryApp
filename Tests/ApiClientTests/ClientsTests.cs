using Common.ClientBuilder;
using TelemetryApp.Api.Client.ApiTelemetry;
using TelemetryApp.Api.Client.Log;

namespace ApiClientTests;

public class ClientsTests
{
    [SetUp]
    public void Setup()
    {
        apiTelemetryClient = ApiTelemetryClientBuilder.Build();
        loggerClient = LoggerClientBuilder.Build();
    }

    //[Test]
    public async Task TestApiTelemetryClient()
    {
        await apiTelemetryClient.CreateAsync("GET", "/", 200, 228);
    }

    //[Test]
    public async Task TestLoggerClient()
    {
        await loggerClient.InfoAsync("Test {a} {b} {c}", 1, 2, 3);
    }

    private IApiTelemetryClient apiTelemetryClient;
    private ILoggerClient loggerClient;
}