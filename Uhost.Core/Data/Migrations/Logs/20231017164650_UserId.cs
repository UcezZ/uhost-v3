using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations.Logs
{
    public partial class UserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvokerId",
                table: "Logs",
                newName: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "InvokerId");
        }
    }
}
