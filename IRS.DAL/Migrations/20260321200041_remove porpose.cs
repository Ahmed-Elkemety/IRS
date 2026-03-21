using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class removeporpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_otps_Email_Purpose",
                table: "otps");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "otps");

            migrationBuilder.CreateIndex(
                name: "IX_otps_Email",
                table: "otps",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_otps_Email",
                table: "otps");

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "otps",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_otps_Email_Purpose",
                table: "otps",
                columns: new[] { "Email", "Purpose" });
        }
    }
}
