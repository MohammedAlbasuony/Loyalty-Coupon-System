using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyCouponsSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class deletUnnessaryAttributeInCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponSort",
                table: "Coupons");

          

            migrationBuilder.DropColumn(
                name: "CouponType",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Coupons");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Governate",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Governate",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "CouponSort",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);

            

            migrationBuilder.AddColumn<string>(
                name: "CouponType",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Coupons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
