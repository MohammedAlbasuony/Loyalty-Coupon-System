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
            migrationBuilder.DropForeignKey(
                name: "FK_DistributorCustomer_Distributors_DistributorID",
                table: "DistributorCustomer");

            migrationBuilder.AddForeignKey(
                name: "FK_DistributorCustomer_Distributors_DistributorID",
                table: "DistributorCustomer",
                column: "DistributorID",
                principalTable: "Distributors",
                principalColumn: "DistributorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistributorCustomer_Distributors_DistributorID",
                table: "DistributorCustomer");

            migrationBuilder.AddForeignKey(
                name: "FK_DistributorCustomer_Distributors_DistributorID",
                table: "DistributorCustomer",
                column: "DistributorID",
                principalTable: "Distributors",
                principalColumn: "DistributorID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
