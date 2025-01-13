using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyCouponsSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DistributorCreatedByAtDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Distributors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Distributors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Distributors",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
