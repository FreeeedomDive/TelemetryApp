using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace TelemetryApp.Core.ApiTelemetry.Repository;

[PrimaryKey("Id", "Project", "Service")]
[Index("DateTime")]
public class ApiTelemetryStorageElement : SqlStorageElement
{
    [Key] public string Project { get; set; }
    [Key] public string Service { get; set; }
    public string Method { get; set; }
    public string Route { get; set; }
    public int StatusCode { get; set; }
    public long ExecutionTime { get; set; }
    public DateTime DateTime { get; set; }
}