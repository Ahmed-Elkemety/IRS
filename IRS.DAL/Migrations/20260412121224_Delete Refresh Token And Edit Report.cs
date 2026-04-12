using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteRefreshTokenAndEditReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refreshTokens");

            migrationBuilder.AddColumn<int>(
                name: "Periority",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Periority",
                table: "Reports");

            migrationBuilder.CreateTable(
                name: "refreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_refreshTokens_Token",
                table: "refreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refreshTokens_UserId",
                table: "refreshTokens",
                column: "UserId");
        }
    }
}
