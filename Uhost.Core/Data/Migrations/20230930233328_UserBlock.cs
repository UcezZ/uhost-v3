using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations
{
    public partial class UserBlock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedAt",
                table: "Users",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlockedByUserId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BlockedByUserId",
                table: "Users",
                column: "BlockedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_BlockedByUserId",
                table: "Users",
                column: "BlockedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_BlockedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BlockedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlockedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlockedByUserId",
                table: "Users");
        }
    }
}
