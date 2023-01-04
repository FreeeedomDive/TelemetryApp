using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TelemetryApp.Core.ApiTelemetry.Repository;
using TelemetryApp.Core.Logs.Repository;
using TelemetryApp.Core.ProjectServices.Repository;

namespace TelemetryApp.Core.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(
        DbContextOptions<DatabaseContext> options,
        IOptions<DatabaseOptions> dbOptionsAccessor
    ) : base(options)
    {
        Options = dbOptionsAccessor.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Options.ConnectionString);
    }

    public DatabaseOptions Options { get; }
    public DbSet<ProjectServiceStorageElement> ProjectServicesStorage { get; set; }
    public DbSet<ApiTelemetryStorageElement> ApiTelemetryStorage { get; set; }
    public DbSet<LogStorageElement> LogsStorage { get; set; }
}