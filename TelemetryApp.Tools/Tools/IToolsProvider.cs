namespace TelemetryApp.Tools.Tools;

public interface IToolsProvider
{
    ITool[] GetAll();
    ITool? FindByName(string name);
}