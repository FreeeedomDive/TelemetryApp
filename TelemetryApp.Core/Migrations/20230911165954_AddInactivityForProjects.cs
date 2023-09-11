using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryApp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddInactivityForProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInactive",
                table: "ProjectServicesStorage",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInactive",
                table: "ProjectServicesStorage");
        }
    }
}
