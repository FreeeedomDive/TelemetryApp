using TelemetryApp.Sentry.Service;

namespace Common.Mocks;

public class ExceptionEventsSentryServiceMock : IExceptionEventsSentryService
{
    public void CaptureException(Exception exception)
    {
        
    }
}