namespace TelemetryApp.Tools.Tools;

public class ToolsProvider : IToolsProvider
{
    public ToolsProvider(ITool[] tools)
    {
        this.tools = tools;
    }

    public ITool[] GetAll()
    {
        return tools;
    }

    public ITool? FindByName(string name)
    {
        return tools.FirstOrDefault(x => x.Name == name);
    }

    private readonly ITool[] tools;
}