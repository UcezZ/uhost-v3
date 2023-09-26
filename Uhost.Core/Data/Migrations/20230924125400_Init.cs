using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Uhost.Core.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_random_chars(len INT)
RETURNS TEXT
AS $$
DECLARE val TEXT;
BEGIN
    SELECT
        string_agg(SUBSTR(characters, (RANDOM()* LENGTH(characters) + 1)::integer, 1), '') AS random_word
    FROM(VALUES('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_')) AS symbols(characters)
        JOIN generate_series(1, len) on 1 = 1
    INTO val;
    
    RETURN val;
END
$$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_video_token()
RETURNS TEXT
AS $$
DECLARE token TEXT;
BEGIN
    SELECT gen_random_chars(16) INTO token;

    IF EXISTS (SELECT ""Token"" FROM ""Videos"" WHERE ""Token"" = token) 
    THEN
        RETURN gen_video_token();
    ELSE
        RETURN token;
    END IF;
END
$$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_file_token()
RETURNS TEXT
AS $$
DECLARE token TEXT;
BEGIN
    SELECT gen_random_chars(32) INTO token;

    IF EXISTS (SELECT ""Token"" FROM ""Files"" WHERE ""Token"" = token) 
    THEN
        RETURN gen_file_token();
    ELSE
        RETURN token;
    END IF;
END
$$ LANGUAGE plpgsql;");

            migrationBuilder.CreateTable(
                name: "Rights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Desctiption = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Login = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Password = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Theme = table.Column<string>(type: "varchar(8)", nullable: false, defaultValue: ""),
                    LastVisitAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleRights",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    RightId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRights", x => new { x.RightId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RoleRights_Rights_RightId",
                        column: x => x.RightId,
                        principalTable: "Rights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleRights_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false, defaultValue: "file.bin"),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    Mime = table.Column<string>(type: "text", nullable: false, defaultValue: "application/octet-stream"),
                    Type = table.Column<string>(type: "varchar(16)", nullable: true, defaultValue: ""),
                    DynId = table.Column<int>(type: "integer", nullable: true),
                    DynName = table.Column<string>(type: "varchar(16)", nullable: true, defaultValue: ""),
                    Digest = table.Column<string>(type: "char(40)", nullable: false),
                    Token = table.Column<string>(type: "char(32)", nullable: false, defaultValueSql: "gen_file_token()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Description = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Token = table.Column<string>(type: "char(16)", nullable: false, defaultValueSql: "gen_video_token()"),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_Token",
                table: "Files",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_UserId",
                table: "Files",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rights_Name",
                table: "Rights",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleRights_RoleId",
                table: "RoleRights",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Token",
                table: "Videos",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_UserId",
                table: "Videos",
                column: "UserId");

            migrationBuilder.Sql("CREATE INDEX \"IX_Files_Type\" ON \"Files\" USING GIN(\"Type\" gin_trgm_ops)");
            migrationBuilder.Sql("CREATE INDEX \"IX_Files_DynName\" ON \"Files\" USING GIN(\"DynName\" gin_trgm_ops)");

            migrationBuilder.Sql("CREATE INDEX \"IX_Videos_Name\" ON \"Videos\" USING GIN(\"Name\" gin_trgm_ops)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "RoleRights");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Rights");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS gen_file_token()");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS gen_video_token()");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS gen_random_chars(len INT)");
        }
    }
}
