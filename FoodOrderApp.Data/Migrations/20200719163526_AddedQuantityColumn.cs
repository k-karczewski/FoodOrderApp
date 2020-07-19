using Microsoft.EntityFrameworkCore.Migrations;

namespace FoodOrderApp.Data.Migrations
{
    public partial class AddedQuantityColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "PizzaOrders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "PizzaOrders");
        }
    }
}
