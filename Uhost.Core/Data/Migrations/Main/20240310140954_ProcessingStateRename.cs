using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace Uhost.Core.Data.Migrations.Main
{
    public partial class ProcessingStateRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE TEMP TABLE temp_vcs AS SELECT * FROM \"VideoConversionStates\"");

            migrationBuilder.DropTable(
                name: "VideoConversionStates");

            migrationBuilder.CreateTable(
                name: "VideoProcessingStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VideoId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false, defaultValue: ""),
                    State = table.Column<string>(type: "TEXT", nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoProcessingStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoProcessingStates_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoProcessingStates_VideoId",
                table: "VideoProcessingStates",
                column: "VideoId");

            migrationBuilder.Sql("INSERT INTO \"VideoProcessingStates\" SELECT * FROM temp_vcs");
            migrationBuilder.Sql("DROP TABLE IF EXISTS temp_vcs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE TEMP TABLE temp_vcs AS SELECT * FROM \"VideoProcessingStates\"");

            migrationBuilder.DropTable(
                name: "VideoProcessingStates");

            migrationBuilder.CreateTable(
                name: "VideoConversionStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: false, defaultValue: ""),
                    Type = table.Column<string>(type: "TEXT", nullable: false, defaultValue: ""),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    VideoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoConversionStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoConversionStates_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoConversionStates_VideoId",
                table: "VideoConversionStates",
                column: "VideoId");

            migrationBuilder.Sql("INSERT INTO \"VideoConversionStates\" SELECT * FROM temp_vcs");
            migrationBuilder.Sql("DROP TABLE IF EXISTS temp_vcs");
        }
    }
}
