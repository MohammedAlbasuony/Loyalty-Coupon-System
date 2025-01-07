using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyCouponsSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQRCodeAssgnmentEntty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TechnicianID",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "TechnicianID",
                table: "Transactions");
        }
    }
}
