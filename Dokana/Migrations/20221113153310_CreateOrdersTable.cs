using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokana.Migrations
{
    public partial class CreateOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoreAboutAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    PaymentMethodFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    DateOfConfirmation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsShipping = table.Column<bool>(type: "bit", nullable: false),
                    DateOfShipping = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    DateOfDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BuyerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BuyerId",
                table: "Orders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShoppingCartId",
                table: "Orders",
                column: "ShoppingCartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
