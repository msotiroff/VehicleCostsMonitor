namespace VehicleCostsMonitor.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CostsSumRemovedFromVehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalFuelCosts",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalOtherCosts",
                table: "Vehicles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
