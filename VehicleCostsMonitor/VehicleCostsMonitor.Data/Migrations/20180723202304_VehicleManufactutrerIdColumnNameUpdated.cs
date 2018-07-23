using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleCostsMonitor.Data.Migrations
{
    public partial class VehicleManufactutrerIdColumnNameUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ManufactureId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Vehicles",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Vehicles",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ManufactureId",
                table: "Vehicles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerId",
                table: "Vehicles",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
