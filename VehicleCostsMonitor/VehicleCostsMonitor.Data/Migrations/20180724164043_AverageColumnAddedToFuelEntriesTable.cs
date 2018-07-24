using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleCostsMonitor.Data.Migrations
{
    public partial class AverageColumnAddedToFuelEntriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Average",
                table: "FuelEntries",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Average",
                table: "FuelEntries");
        }
    }
}
