using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyCouponsSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponSort",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponType",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExchangePermission",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Governate",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianID",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Technicians",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TechnicianID",
                table: "Transactions",
                column: "TechnicianID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Technicians_TechnicianID",
                table: "Transactions",
                column: "TechnicianID",
                principalTable: "Technicians",
                principalColumn: "TechnicianID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Technicians_TechnicianID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TechnicianID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CouponSort",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CouponType",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ExchangePermission",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Governate",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TechnicianID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Customers");
        }
    }
}
