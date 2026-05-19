using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buildwave.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoutAt",
                table: "user_sessions");

            migrationBuilder.RenameColumn(
                name: "LoginAt",
                table: "user_sessions",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "LastActivityAt",
                table: "user_sessions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "user_sessions",
                newName: "IsRevoked");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "user_sessions",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "user_sessions",
                newName: "LoginAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "user_sessions",
                newName: "LastActivityAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogoutAt",
                table: "user_sessions",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
