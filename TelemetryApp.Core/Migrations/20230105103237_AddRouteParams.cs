using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryApp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RouteValues",
                table: "ApiTelemetryStorage",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteValues",
                table: "ApiTelemetryStorage");
        }
    }
}
