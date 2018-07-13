using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleCostsMonitor.Data.Migrations
{
    public partial class DateColumnAddedToUserActivityLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "UserActivityLogs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "UserActivityLogs");
        }
    }
}
