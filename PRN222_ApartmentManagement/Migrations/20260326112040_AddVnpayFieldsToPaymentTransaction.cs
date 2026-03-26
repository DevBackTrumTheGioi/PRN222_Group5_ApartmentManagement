using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddVnpayFieldsToPaymentTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VnpBankCode",
                table: "PaymentTransactions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnpPayDate",
                table: "PaymentTransactions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnpResponseCode",
                table: "PaymentTransactions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnpTransactionNo",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnpTxnRef",
                table: "PaymentTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_VnpTxnRef",
                table: "PaymentTransactions",
                column: "VnpTxnRef");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_VnpTxnRef",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "VnpBankCode",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "VnpPayDate",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "VnpResponseCode",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "VnpTransactionNo",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "VnpTxnRef",
                table: "PaymentTransactions");
        }
    }
}
