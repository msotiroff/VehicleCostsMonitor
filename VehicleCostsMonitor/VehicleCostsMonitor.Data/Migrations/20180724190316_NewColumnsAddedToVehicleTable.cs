using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleCostsMonitor.Data.Migrations
{
    public partial class NewColumnsAddedToVehicleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalDistance",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalFuelAmount",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFuelCosts",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOtherCosts",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDistance",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalFuelAmount",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalFuelCosts",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalOtherCosts",
                table: "Vehicles");
        }
    }
}
