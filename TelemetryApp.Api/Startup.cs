using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Configuration.Extensions;
using TelemetryApp.Core.ApiTelemetry.Repository;
using TelemetryApp.Core.ApiTelemetry.Repository.Elastic;
using TelemetryApp.Core.ApiTelemetry.Repository.PostgreSql;
using TelemetryApp.Core.ApiTelemetry.Service;
using TelemetryApp.Core.Database;
using TelemetryApp.Core.Elastic;
using TelemetryApp.Core.Logs.Repository;
using TelemetryApp.Core.Logs.Repository.Elastic;
using TelemetryApp.Core.Logs.Repository.PostgreSql;
using TelemetryApp.Core.Logs.Service;
using TelemetryApp.Core.ProjectServices.Repository;

namespace TelemetryApp.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var postgreSqlConfigurationSection = Configuration.GetSection("PostgreSql");
        services.Configure<DatabaseOptions>(postgreSqlConfigurationSection);
        var elasticSqlConfigurationSection = Configuration.GetSection("Elastic");
        services.Configure<ElasticOptions>(elasticSqlConfigurationSection);
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);

        services.ConfigurePostgreSql();

        services.AddSingleton<ElasticsearchClient>(
            provider =>
            {
                var options = provider.GetService<IOptions<ElasticOptions>>()!.Value;
                var settings = new ElasticsearchClientSettings(new Uri(options.ConnectionString))
                    .Authentication(new BasicAuthentication(options.ConnectionUserName, options.ConnectionPassword));
                return new ElasticsearchClient(settings);
            }
        );

        services.AddTransient<IProjectServiceRepository, ProjectServiceRepository>();
        services.AddTransient<SqlApiTelemetryRepository>();
        services.AddTransient<ElasticApiTelemetryRepository>();
        services.AddTransient<IApiTelemetryRepository, SqlApiTelemetryRepository>();
        services.AddTransient<SqlLogRepository>();
        services.AddTransient<ElasticLogRepository>();
        services.AddTransient<ILogRepository, SqlLogRepository>();

        services.AddTransient<IApiTelemetryService, ApiTelemetryService>();
        services.AddTransient<ILogService, LogService>();

        services.AddCors(
            options =>
            {
                options.AddPolicy(
                    CorsConfigurationName,
                    policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); }
                );
            }
        );

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors(CorsConfigurationName);
        app.UseWebSockets();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public IConfiguration Configuration { get; }

    private const string CorsConfigurationName = "AllowOrigins";
}