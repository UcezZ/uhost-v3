using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations
{
    public partial class FileDigestLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Digest",
                table: "Files",
                type: "char(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Digest",
                table: "Files",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(32)");
        }
    }
}
