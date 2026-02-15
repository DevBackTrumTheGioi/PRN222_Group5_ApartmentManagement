using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class MergeResidentIntoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // BEFORE dropping the Residents table, we should migrate the data
            // But EF Core generated DropTable first. I should move it.
            
            migrationBuilder.DropForeignKey(
                name: "FK_AmenityBookings_Residents_ResidentId",
                table: "AmenityBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractMembers_Residents_ResidentId",
                table: "ContractMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceAuthHistory_Residents_ResidentId",
                table: "FaceAuthHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Residents_ResidentId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_ResidentCards_Residents_ResidentId",
                table: "ResidentCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Residents_ResidentId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Residents_ResidentId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_Apartments_ApartmentId",
                table: "Visitors");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_Residents_RegisteredBy",
                table: "Visitors");

            // Add columns to Users first
            migrationBuilder.AddColumn<int>(
                name: "ApartmentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaceDescriptor",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityCardNumber",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFaceRegistered",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MoveInDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MoveOutDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResidencyStatus",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResidentType",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // DATA MIGRATION SCRIPT
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.DateOfBirth = r.DateOfBirth,
                    u.IdentityCardNumber = r.IdentityCardNumber,
                    u.ResidentType = r.ResidentType,
                    u.ResidencyStatus = r.ResidencyStatus,
                    u.ApartmentId = r.ApartmentId,
                    u.MoveInDate = r.MoveInDate,
                    u.MoveOutDate = r.MoveOutDate,
                    u.Note = r.Note,
                    u.FaceDescriptor = r.FaceDescriptor,
                    u.IsFaceRegistered = r.IsFaceRegistered
                FROM Users u
                INNER JOIN Residents r ON u.UserId = r.UserId
            ");

            migrationBuilder.DropTable(
                name: "Residents");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApartmentId",
                table: "Users",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityCardNumber",
                table: "Users",
                column: "IdentityCardNumber",
                unique: true,
                filter: "[IdentityCardNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AmenityBookings_Users_ResidentId",
                table: "AmenityBookings",
                column: "ResidentId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMembers_Users_ResidentId",
                table: "ContractMembers",
                column: "ResidentId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FaceAuthHistory_Users_ResidentId",
                table: "FaceAuthHistory",
                column: "ResidentId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_ResidentId",
                table: "Requests",
                column: "ResidentId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResidentCards_Users_ResidentId",
                table: "ResidentCards",
                column: "ResidentId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Users_ResidentId",
                table: "ServiceOrders",
                column: "ResidentId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Apartments_ApartmentId",
                table: "Users",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "ApartmentId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Users_ResidentId",
                table: "Vehicles",
                column: "ResidentId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_Apartments_ApartmentId",
                table: "Visitors",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "ApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_Users_RegisteredBy",
                table: "Visitors",
                column: "RegisteredBy",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AmenityBookings_Users_ResidentId",
                table: "AmenityBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractMembers_Users_ResidentId",
                table: "ContractMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceAuthHistory_Users_ResidentId",
                table: "FaceAuthHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_ResidentId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_ResidentCards_Users_ResidentId",
                table: "ResidentCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Users_ResidentId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Apartments_ApartmentId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Users_ResidentId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_Apartments_ApartmentId",
                table: "Visitors");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_Users_RegisteredBy",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Users_ApartmentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityCardNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FaceDescriptor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IdentityCardNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsFaceRegistered",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MoveInDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MoveOutDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResidencyStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResidentType",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ApartmentId = table.Column<int>(type: "int", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FaceDescriptor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentityCardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsFaceRegistered = table.Column<bool>(type: "bit", nullable: false),
                    MoveInDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoveOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResidencyStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ResidentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Residents_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "ApartmentId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Residents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Residents_ApartmentId",
                table: "Residents",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Residents_IdentityCardNumber",
                table: "Residents",
                column: "IdentityCardNumber",
                unique: true,
                filter: "[IdentityCardNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AmenityBookings_Residents_ResidentId",
                table: "AmenityBookings",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractMembers_Residents_ResidentId",
                table: "ContractMembers",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceAuthHistory_Residents_ResidentId",
                table: "FaceAuthHistory",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Residents_ResidentId",
                table: "Requests",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResidentCards_Residents_ResidentId",
                table: "ResidentCards",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Residents_ResidentId",
                table: "ServiceOrders",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Residents_ResidentId",
                table: "Vehicles",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_Apartments_ApartmentId",
                table: "Visitors",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "ApartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_Residents_RegisteredBy",
                table: "Visitors",
                column: "RegisteredBy",
                principalTable: "Residents",
                principalColumn: "UserId");
        }
    }
}
