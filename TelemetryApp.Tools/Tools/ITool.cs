namespace TelemetryApp.Tools.Tools;

public interface ITool
{
    string Name { get; }
    Task RunAsync();
}