using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.Net;

namespace Uhost.Core.Data.Migrations.Logs
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    InvokerId = table.Column<int>(type: "integer", nullable: true),
                    Data = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::JSONB"),
                    IPAddress = table.Column<IPAddress>(type: "inet", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateIndex("IX_Logs_EventId", "Logs", "EventId");
            migrationBuilder.CreateIndex("IX_Logs_CreatedAt", "Logs", "CreatedAt");
            migrationBuilder.CreateIndex("IX_Logs_InvokerId", "Logs", "InvokerId");
            migrationBuilder.CreateIndex("IX_Logs_IPAddress", "Logs", "IPAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
