using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Ninject;
using TelemetryApp.Core.Database;
using TelemetryApp.Core.Elastic;
using TelemetryApp.Tools.Tools;

namespace TelemetryApp.Tools.Dependencies;

public static class DependencyBuilder
{
    public static StandardKernel Build()
    {
        return new StandardKernel()
            .ConfigureSettings()
            .ConfigureElasticClient()
            .ConfigurePostgreSqlContext()
            .ConfigureTools();
    }

    private static StandardKernel ConfigureSettings(this StandardKernel kernel)
    {
        // Don't commit this settings
        var elasticOptions = new ElasticOptions
        {
            ConnectionString = "",
            ConnectionUserName = "",
            ConnectionPassword = "",
            ApplicationName = "",
            Environment = ""
        };

        kernel.Bind<ElasticOptions>().ToConstant(elasticOptions);

        return kernel;
    }

    private static StandardKernel ConfigureElasticClient(this StandardKernel kernel)
    {
        var elasticOptions = kernel.Get<ElasticOptions>();
        var clientSettings = new ElasticsearchClientSettings(new Uri(elasticOptions.ConnectionString))
            .Authentication(new BasicAuthentication(elasticOptions.ConnectionUserName, elasticOptions.ConnectionPassword));
        var elasticClient = new ElasticsearchClient(clientSettings);
        
        kernel.Bind<ElasticsearchClient>().ToConstant(elasticClient);

        return kernel;
    }

    private static StandardKernel ConfigurePostgreSqlContext(this StandardKernel kernel)
    {
        // Ensure that you have "TelemetryApp.Tests.ConnectionString" environment variable
        var databaseContext = new DatabaseContext();
        
        kernel.Bind<DatabaseContext>().ToConstant(databaseContext);

        return kernel;
    }

    private static StandardKernel ConfigureTools(this StandardKernel kernel)
    {
        var tools = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(ITool).IsAssignableFrom(x) && !x.IsInterface)
            .ToArray();

        foreach (var tool in tools)
        {
            kernel.Bind<ITool>().To(tool);
        }

        kernel.Bind<IToolsProvider>().To<ToolsProvider>();

        return kernel;
    }
}