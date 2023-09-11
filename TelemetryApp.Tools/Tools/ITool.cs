namespace TelemetryApp.Tools.Tools;

public interface ITool
{
    Task RunAsync();
    string Name { get; }
}