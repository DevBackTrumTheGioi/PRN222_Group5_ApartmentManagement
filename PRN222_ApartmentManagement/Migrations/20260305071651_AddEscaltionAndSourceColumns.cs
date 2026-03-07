using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEscaltionAndSourceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EscalatedAt",
                table: "Requests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscalatedTo",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EscalationReason",
                table: "Requests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Announcements",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_EscalatedTo",
                table: "Requests",
                column: "EscalatedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_EscalatedTo",
                table: "Requests",
                column: "EscalatedTo",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_EscalatedTo",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_EscalatedTo",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "EscalatedAt",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "EscalatedTo",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "EscalationReason",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Announcements");
        }
    }
}
