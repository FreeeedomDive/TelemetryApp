using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace TelemetryApp.Core.ProjectServices.Repository;

[Index("Project", "Service")]
public class ProjectServiceStorageElement : SqlStorageElement
{
    public string Project { get; set; }
    public string Service { get; set; }
}