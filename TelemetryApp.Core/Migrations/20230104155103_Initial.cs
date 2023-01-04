using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryApp.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiTelemetryStorage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Project = table.Column<string>(type: "text", nullable: false),
                    Service = table.Column<string>(type: "text", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    Route = table.Column<string>(type: "text", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    ExecutionTime = table.Column<long>(type: "bigint", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTelemetryStorage", x => new { x.Id, x.Project, x.Service });
                });

            migrationBuilder.CreateTable(
                name: "LogsStorage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Project = table.Column<string>(type: "text", nullable: false),
                    Service = table.Column<string>(type: "text", nullable: false),
                    LogLevel = table.Column<string>(type: "text", nullable: false),
                    Template = table.Column<string>(type: "text", nullable: false),
                    Params = table.Column<string>(type: "text", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsStorage", x => new { x.Id, x.Project, x.Service });
                });

            migrationBuilder.CreateTable(
                name: "ProjectServicesStorage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Project = table.Column<string>(type: "text", nullable: false),
                    Service = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServicesStorage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiTelemetryStorage_DateTime",
                table: "ApiTelemetryStorage",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_LogsStorage_LogLevel_DateTime",
                table: "LogsStorage",
                columns: new[] { "LogLevel", "DateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServicesStorage_Project_Service",
                table: "ProjectServicesStorage",
                columns: new[] { "Project", "Service" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiTelemetryStorage");

            migrationBuilder.DropTable(
                name: "LogsStorage");

            migrationBuilder.DropTable(
                name: "ProjectServicesStorage");
        }
    }
}
