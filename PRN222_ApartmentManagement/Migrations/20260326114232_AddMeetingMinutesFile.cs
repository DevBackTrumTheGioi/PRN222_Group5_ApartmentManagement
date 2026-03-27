using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingMinutesFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MinutesFileName",
                table: "Meetings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinutesFilePath",
                table: "Meetings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutesFileName",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "MinutesFilePath",
                table: "Meetings");
        }
    }
}
