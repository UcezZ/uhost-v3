using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations.Main
{
    public partial class VideoLoopPlayback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LoopPlayback",
                table: "Videos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoopPlayback",
                table: "Videos");
        }
    }
}
