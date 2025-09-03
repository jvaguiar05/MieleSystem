using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MieleSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAuditLogs",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserPublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Email = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    OccurredAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuditLogs", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(
                        type: "character varying(150)",
                        maxLength: 150,
                        nullable: false
                    ),
                    Email = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    PasswordHash = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    RegistrationSituation = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "OtpSessions",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    OtpCode = table.Column<string>(
                        type: "character varying(6)",
                        maxLength: 6,
                        nullable: false
                    ),
                    OtpExpiresAt = table.Column<DateTime>(
                        type: "timestamp without time zone",
                        nullable: false
                    ),
                    CreatedAtUtc = table.Column<DateTime>(
                        type: "timestamp without time zone",
                        nullable: false
                    ),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    Purpose = table.Column<int>(type: "integer", nullable: false),
                    UsedAtUtc = table.Column<DateTime>(
                        type: "timestamp without time zone",
                        nullable: true
                    ),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    TokenValue = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    ExpiresAtUtc = table.Column<DateTime>(
                        type: "timestamp without time zone",
                        nullable: false
                    ),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(
                        type: "timestamp without time zone",
                        nullable: true
                    ),
                    CreatedAtUtc = table.Column<DateTime>(
                        type: "timestamp without time zone",
                        nullable: false
                    ),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_OtpSessions_PublicId",
                table: "OtpSessions",
                column: "PublicId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_OtpSessions_UserId",
                table: "OtpSessions",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PublicId",
                table: "RefreshTokens",
                column: "PublicId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserAuditLogs_UserId",
                table: "UserAuditLogs",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserAuditLogs_UserPublicId",
                table: "UserAuditLogs",
                column: "UserPublicId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Users_PublicId",
                table: "Users",
                column: "PublicId",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OtpSessions");

            migrationBuilder.DropTable(name: "RefreshTokens");

            migrationBuilder.DropTable(name: "UserAuditLogs");

            migrationBuilder.DropTable(name: "Users");
        }
    }
}
