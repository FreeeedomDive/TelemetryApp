using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace TelemetryApp.Core.Logs.Repository.PostgreSql;

[PrimaryKey("Id", "Project", "Service"), Index("LogLevel", "DateTime")]
public class LogStorageElement : SqlStorageElement
{
    [Key] public string Project { get; set; }
    [Key] public string Service { get; set; }
    public string LogLevel { get; set; }
    public string Template { get; set; }
    public string Params { get; set; }
    public string Exception { get; set; }
    public DateTime DateTime { get; set; }
}