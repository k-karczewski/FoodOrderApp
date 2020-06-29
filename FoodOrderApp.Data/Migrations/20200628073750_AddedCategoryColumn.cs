using Microsoft.EntityFrameworkCore.Migrations;

namespace FoodOrderApp.Data.Migrations
{
    public partial class AddedCategoryColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Pizzas",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Pizzas");
        }
    }
}
