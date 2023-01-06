namespace TelemetryApp.Api.Client;

public abstract class BaseClient
{
    protected BaseClient(bool exceptionsSafe = true)
    {
        this.exceptionsSafe = exceptionsSafe;
    }

    public async Task PerformRequestAsync(Func<Task> request)
    {
        if (exceptionsSafe)
        {
            try
            {
                await request();
            }
            catch
            {
                // ignore
            }
        }
        else
        {
            await request();
        }
    }

    public async Task<T> PerformRequestAsync<T>(Func<Task<T>> request)
    {
        if (!exceptionsSafe)
        {
            return await request();
        }

        try
        {
            return await request();
        }
        catch
        {
            return default!;
        }
    }

    private readonly bool exceptionsSafe;
}