using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BbibbJobStreetJwtToken.Migrations
{
    /// <inheritdoc />
    public partial class updatedbcompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "Perusahaans",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiredAt",
                table: "Perusahaans",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 1, 10, 55, 29, 612, DateTimeKind.Local).AddTicks(4524), "$2a$11$.h4Xf216sGUEea4E3Iw8zuhfrO6zk3yUTfB0M6J.duSTTAx/kUWQe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "Perusahaans");

            migrationBuilder.DropColumn(
                name: "OtpExpiredAt",
                table: "Perusahaans");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 8, 31, 22, 43, 41, 558, DateTimeKind.Local).AddTicks(5399), "$2a$11$oHFVt0YWDuUCFHJilUF7Eu5PEGSDAVRXtCXaHEzf6nTbREz0ebeVS" });
        }
    }
}
