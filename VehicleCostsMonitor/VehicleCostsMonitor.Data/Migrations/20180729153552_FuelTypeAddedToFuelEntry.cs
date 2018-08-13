namespace VehicleCostsMonitor.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class FuelTypeAddedToFuelEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FuelTypeId",
                table: "FuelEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FuelEntries_FuelTypeId",
                table: "FuelEntries",
                column: "FuelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelEntries_FuelTypes_FuelTypeId",
                table: "FuelEntries",
                column: "FuelTypeId",
                principalTable: "FuelTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelEntries_FuelTypes_FuelTypeId",
                table: "FuelEntries");

            migrationBuilder.DropIndex(
                name: "IX_FuelEntries_FuelTypeId",
                table: "FuelEntries");

            migrationBuilder.DropColumn(
                name: "FuelTypeId",
                table: "FuelEntries");
        }
    }
}
