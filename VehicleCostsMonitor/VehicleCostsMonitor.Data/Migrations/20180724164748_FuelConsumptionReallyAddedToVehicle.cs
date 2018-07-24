using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleCostsMonitor.Data.Migrations
{
    public partial class FuelConsumptionReallyAddedToVehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FuelConsumption",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FuelConsumption",
                table: "Vehicles");
        }
    }
}
