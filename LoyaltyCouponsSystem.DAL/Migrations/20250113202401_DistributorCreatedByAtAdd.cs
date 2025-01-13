using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyCouponsSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DistributorCreatedByAtAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Distributors",
                type: "datetime2",
                nullable: true);

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
                name: "CreatedAt",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Distributors");
        }
    }
}
