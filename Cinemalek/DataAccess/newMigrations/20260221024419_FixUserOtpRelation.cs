using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinemalek.DataAccess.newMigrations
{
    /// <inheritdoc />
    public partial class FixUserOtpRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
            name: "FK_ApplicationUserOTPs_AspNetUsers_ApplicationUserId",
            table: "ApplicationUserOTPs",
            column: "ApplicationUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
            name: "FK_ApplicationUserOTPs_AspNetUsers_ApplicationUserId",
            table: "ApplicationUserOTPs");
        }
    }
}
