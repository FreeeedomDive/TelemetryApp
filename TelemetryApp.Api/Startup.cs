using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Configuration.Extensions;
using TelemetryApp.Core.ApiTelemetry.Repository;
using TelemetryApp.Core.ApiTelemetry.Service;
using TelemetryApp.Core.Database;
using TelemetryApp.Core.Logs.Repository;
using TelemetryApp.Core.Logs.Service;
using TelemetryApp.Core.ProjectServices.Repository;

namespace TelemetryApp.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var postgreSqlConfigurationSection = Configuration.GetSection("PostgreSql");
        services.Configure<DatabaseOptions>(postgreSqlConfigurationSection);
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);

        services.ConfigurePostgreSql();

        services.AddTransient<IProjectServiceRepository, ProjectServiceRepository>();
        services.AddTransient<IApiTelemetryRepository, ApiTelemetryRepository>();
        services.AddTransient<ILogRepository, LogRepository>();

        services.AddTransient<IApiTelemetryService, ApiTelemetryService>();
        services.AddTransient<ILogService, LogService>();
        
        services.AddCors(options =>
        {
            options.AddPolicy(CorsConfigurationName,
                policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
        });

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

    private const string CorsConfigurationName = "AllowOrigins";
}