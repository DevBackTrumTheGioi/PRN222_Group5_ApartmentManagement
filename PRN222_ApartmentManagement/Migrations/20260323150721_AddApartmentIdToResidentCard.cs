using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddApartmentIdToResidentCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApartmentId",
                table: "ResidentCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ResidentCards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ResidentCards_ApartmentId",
                table: "ResidentCards",
                column: "ApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResidentCards_Apartments_ApartmentId",
                table: "ResidentCards",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "ApartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResidentCards_Apartments_ApartmentId",
                table: "ResidentCards");

            migrationBuilder.DropIndex(
                name: "IX_ResidentCards_ApartmentId",
                table: "ResidentCards");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "ResidentCards");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ResidentCards");
        }
    }
}
