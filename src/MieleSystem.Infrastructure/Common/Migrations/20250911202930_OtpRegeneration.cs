using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MieleSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OtpRegeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRegeneratedAtUtc",
                table: "OtpSessions",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegenerationAttempts",
                table: "OtpSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRegeneratedAtUtc",
                table: "OtpSessions");

            migrationBuilder.DropColumn(
                name: "RegenerationAttempts",
                table: "OtpSessions");
        }
    }
}
