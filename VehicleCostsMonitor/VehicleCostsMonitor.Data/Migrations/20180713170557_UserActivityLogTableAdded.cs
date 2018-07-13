using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleCostsMonitor.Data.Migrations
{
    public partial class UserActivityLogTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserEmail = table.Column<string>(nullable: false),
                    HttpMethod = table.Column<string>(nullable: false),
                    ControllerName = table.Column<string>(nullable: false),
                    ActionName = table.Column<string>(nullable: false),
                    AreaName = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    QueryString = table.Column<string>(nullable: true),
                    ActionArguments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_UserEmail",
                table: "UserActivityLogs",
                column: "UserEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserActivityLogs");
        }
    }
}
