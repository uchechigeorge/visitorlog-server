using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorLog.Server.Migrations
{
    public partial class Staff_AddColumn_Degree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Degree",
                table: "Staff",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                table: "Staff");
        }
    }
}
