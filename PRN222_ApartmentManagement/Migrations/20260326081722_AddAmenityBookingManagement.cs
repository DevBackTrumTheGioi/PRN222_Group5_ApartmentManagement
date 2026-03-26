using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAmenityBookingManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AmenityBookings_AmenityId",
                table: "AmenityBookings");

            migrationBuilder.AddColumn<int>(
                name: "ParticipantCount",
                table: "AmenityBookings",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CancellationDeadlineHours",
                table: "Amenities",
                type: "int",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CloseTime",
                table: "Amenities",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 22, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OpenTime",
                table: "Amenities",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 6, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "RequiresBooking",
                table: "Amenities",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql("""
                UPDATE AmenityBookings
                SET ParticipantCount = 1
                WHERE ParticipantCount = 0;

                UPDATE AmenityBookings
                SET Status = 'Confirmed'
                WHERE Status = 'Pending';

                UPDATE Amenities
                SET OpenTime = '06:00:00',
                    CloseTime = '22:00:00',
                    CancellationDeadlineHours = 4,
                    RequiresBooking = 1,
                    PricePerHour = ISNULL(PricePerHour, 0);

                UPDATE Amenities
                SET RequiresBooking = 0,
                    PricePerHour = 0,
                    CancellationDeadlineHours = 0
                WHERE AmenityName IN (N'Hồ bơi', N'Phòng Gym', N'Phòng Yoga', N'Khu vui chơi trẻ em');

                UPDATE Amenities
                SET OpenTime = '05:00:00',
                    CloseTime = '21:00:00'
                WHERE AmenityName = N'Hồ bơi';

                UPDATE Amenities
                SET OpenTime = '05:00:00',
                    CloseTime = '22:00:00'
                WHERE AmenityName = N'Phòng Gym';

                UPDATE Amenities
                SET OpenTime = '06:00:00',
                    CloseTime = '21:00:00'
                WHERE AmenityName = N'Phòng Yoga';

                UPDATE Amenities
                SET OpenTime = '07:00:00',
                    CloseTime = '21:00:00'
                WHERE AmenityName = N'Khu vui chơi trẻ em';

                UPDATE Amenities
                SET OpenTime = '09:00:00',
                    CloseTime = '22:00:00',
                    CancellationDeadlineHours = 6
                WHERE AmenityName = N'Phòng BBQ';

                UPDATE Amenities
                SET OpenTime = '08:00:00',
                    CloseTime = '21:00:00',
                    CancellationDeadlineHours = 4
                WHERE AmenityName = N'Phòng Sauna';

                UPDATE Amenities
                SET OpenTime = '07:00:00',
                    CloseTime = '21:00:00',
                    CancellationDeadlineHours = 2
                WHERE AmenityName IN (N'Phòng họp A', N'Phòng họp B');
                """);

            migrationBuilder.CreateIndex(
                name: "IX_AmenityBookings_AmenityId_BookingDate_StartTime_EndTime",
                table: "AmenityBookings",
                columns: new[] { "AmenityId", "BookingDate", "StartTime", "EndTime" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AmenityBookings_AmenityId_BookingDate_StartTime_EndTime",
                table: "AmenityBookings");

            migrationBuilder.DropColumn(
                name: "ParticipantCount",
                table: "AmenityBookings");

            migrationBuilder.DropColumn(
                name: "CancellationDeadlineHours",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "RequiresBooking",
                table: "Amenities");

            migrationBuilder.CreateIndex(
                name: "IX_AmenityBookings_AmenityId",
                table: "AmenityBookings",
                column: "AmenityId");
        }
    }
}
