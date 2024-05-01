using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations.Logs
{
    public partial class LogCodeFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Logs"" 
SET ""EventId"" = 38 
WHERE ""EventId"" = 40");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Logs"" 
SET ""EventId"" = 40 
WHERE ""EventId"" = 38");
        }
    }
}
