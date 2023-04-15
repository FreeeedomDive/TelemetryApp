using Ninject;
using TelemetryApp.Tools.Dependencies;
using TelemetryApp.Tools.Tools;

namespace TelemetryApp.Tools;

public class Program
{
    public static async Task Main(string[] args)
    {
        var diContainer = DependencyBuilder.Build();
        var toolsProvider = diContainer.Get<IToolsProvider>();
        if (args.Length == 0)
        {
            var tools = toolsProvider.GetAll();
            var names = tools.Select(x => x.Name);
            await Console.Error.WriteLineAsync($"Provide a name of tool to launch\nAvailable tools:\n{string.Join("\n", names)}");
            return;
        }

        var toolName = args.First();
        var tool = toolsProvider.FindByName(toolName);
        if (tool is null)
        {
            var tools = toolsProvider.GetAll();
            var names = tools.Select(x => x.Name);
            await Console.Error.WriteLineAsync($"Can't find a tool with name {toolName}\nAvailable tools:\n{string.Join("\n", names)}");
            return;
        }

        await tool.RunAsync();
    }
}