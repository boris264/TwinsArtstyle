using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwinsArtstyle.Infrastructure.Migrations
{
    public partial class addedCompositeKeysToJoinTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrdersProductsCount",
                table: "OrdersProductsCount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartsProductsCount",
                table: "CartsProductsCount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrdersProductsCount",
                table: "OrdersProductsCount",
                columns: new[] { "OrderId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartsProductsCount",
                table: "CartsProductsCount",
                columns: new[] { "CartId", "ProductId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrdersProductsCount",
                table: "OrdersProductsCount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartsProductsCount",
                table: "CartsProductsCount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrdersProductsCount",
                table: "OrdersProductsCount",
                column: "OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartsProductsCount",
                table: "CartsProductsCount",
                column: "CartId");
        }
    }
}
