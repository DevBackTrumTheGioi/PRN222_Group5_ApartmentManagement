using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveParcelMeterReadingAddServiceOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_MeterReadings_MeterReadingId",
                table: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "MeterReadings");

            migrationBuilder.DropTable(
                name: "Parcels");

            migrationBuilder.RenameColumn(
                name: "MeterReadingId",
                table: "InvoiceDetails",
                newName: "ServiceOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDetails_MeterReadingId",
                table: "InvoiceDetails",
                newName: "IX_InvoiceDetails_ServiceOrderId");

            migrationBuilder.CreateTable(
                name: "ServiceOrders",
                columns: table => new
                {
                    ServiceOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApartmentId = table.Column<int>(type: "int", nullable: false),
                    ResidentId = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "date", nullable: false),
                    RequestedTimeSlot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AdditionalCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChargeNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<int>(type: "int", nullable: true),
                    CompletionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    ReviewComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrders", x => x.ServiceOrderId);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "ApartmentId");
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Residents",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ServiceOrders_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "ServiceTypeId");
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ServiceOrders_Users_CompletedBy",
                        column: x => x.CompletedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_ApartmentId",
                table: "ServiceOrders",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_AssignedTo",
                table: "ServiceOrders",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_CompletedBy",
                table: "ServiceOrders",
                column: "CompletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_InvoiceId",
                table: "ServiceOrders",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_OrderNumber",
                table: "ServiceOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_ResidentId",
                table: "ServiceOrders",
                column: "ResidentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_ServiceTypeId",
                table: "ServiceOrders",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_ServiceOrders_ServiceOrderId",
                table: "InvoiceDetails",
                column: "ServiceOrderId",
                principalTable: "ServiceOrders",
                principalColumn: "ServiceOrderId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_ServiceOrders_ServiceOrderId",
                table: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "ServiceOrders");

            migrationBuilder.RenameColumn(
                name: "ServiceOrderId",
                table: "InvoiceDetails",
                newName: "MeterReadingId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDetails_ServiceOrderId",
                table: "InvoiceDetails",
                newName: "IX_InvoiceDetails_MeterReadingId");

            migrationBuilder.CreateTable(
                name: "MeterReadings",
                columns: table => new
                {
                    MeterReadingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartmentId = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    Consumption = table.Column<decimal>(type: "decimal(18,2)", nullable: true, computedColumnSql: "[CurrentReading] - [PreviousReading]", stored: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentReading = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreviousReading = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReadingDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReadings", x => x.MeterReadingId);
                    table.ForeignKey(
                        name: "FK_MeterReadings_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "ApartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeterReadings_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "ServiceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeterReadings_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Parcels",
                columns: table => new
                {
                    ParcelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartmentId = table.Column<int>(type: "int", nullable: false),
                    PickedUpBy = table.Column<int>(type: "int", nullable: true),
                    ReceivedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false),
                    PickedUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sender = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcels", x => x.ParcelId);
                    table.ForeignKey(
                        name: "FK_Parcels_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "ApartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parcels_Residents_PickedUpBy",
                        column: x => x.PickedUpBy,
                        principalTable: "Residents",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Parcels_Users_ReceivedBy",
                        column: x => x.ReceivedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_ApartmentId",
                table: "MeterReadings",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_ServiceTypeId",
                table: "MeterReadings",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_StaffId",
                table: "MeterReadings",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_ApartmentId",
                table: "Parcels",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_PickedUpBy",
                table: "Parcels",
                column: "PickedUpBy");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_ReceivedBy",
                table: "Parcels",
                column: "ReceivedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_TrackingNumber",
                table: "Parcels",
                column: "TrackingNumber",
                unique: true,
                filter: "[TrackingNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_MeterReadings_MeterReadingId",
                table: "InvoiceDetails",
                column: "MeterReadingId",
                principalTable: "MeterReadings",
                principalColumn: "MeterReadingId");
        }
    }
}
