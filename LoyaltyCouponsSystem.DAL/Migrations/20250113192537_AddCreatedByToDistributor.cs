using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyCouponsSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToDistributor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Distributors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Distributors");
        }
    }
}
