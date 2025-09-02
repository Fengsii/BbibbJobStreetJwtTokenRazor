using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BbibbJobStreetJwtToken.Migrations
{
    /// <inheritdoc />
    public partial class updatedbuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiredAt",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "OtpCode", "OtpExpiredAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 8, 31, 22, 43, 41, 558, DateTimeKind.Local).AddTicks(5399), null, null, "$2a$11$oHFVt0YWDuUCFHJilUF7Eu5PEGSDAVRXtCXaHEzf6nTbREz0ebeVS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpExpiredAt",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 8, 21, 11, 58, 22, 993, DateTimeKind.Local).AddTicks(3895), "$2a$11$pH3d4IMhR9lXoAFvsxgyqeFGaerW5M4z4GQYiaEoASVqb7l26.Ylu" });
        }
    }
}
