using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorLog.Server.Migrations
{
    public partial class Admin_AddColumn_AccessId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessId",
                table: "Admins",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessId",
                table: "Admins");
        }
    }
}
