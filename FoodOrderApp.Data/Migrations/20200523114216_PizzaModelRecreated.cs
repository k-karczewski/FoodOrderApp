using Microsoft.EntityFrameworkCore.Migrations;

namespace FoodOrderApp.Data.Migrations
{
    public partial class PizzaModelRecreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pizzas_Starters_StarterId",
                table: "Pizzas");

            migrationBuilder.DropIndex(
                name: "IX_Pizzas_StarterId",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "StarterId",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Pizzas");

            migrationBuilder.CreateTable(
                name: "PizzaPriceModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Size = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PizzaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaPriceModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PizzaPriceModel_Pizzas_PizzaId",
                        column: x => x.PizzaId,
                        principalTable: "Pizzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PizzaStarterModel",
                columns: table => new
                {
                    PizzaId = table.Column<int>(nullable: false),
                    StarterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaStarterModel", x => new { x.PizzaId, x.StarterId });
                    table.ForeignKey(
                        name: "FK_PizzaStarterModel_Pizzas_PizzaId",
                        column: x => x.PizzaId,
                        principalTable: "Pizzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PizzaStarterModel_Starters_StarterId",
                        column: x => x.StarterId,
                        principalTable: "Starters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PizzaPriceModel_PizzaId",
                table: "PizzaPriceModel",
                column: "PizzaId");

            migrationBuilder.CreateIndex(
                name: "IX_PizzaStarterModel_StarterId",
                table: "PizzaStarterModel",
                column: "StarterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PizzaPriceModel");

            migrationBuilder.DropTable(
                name: "PizzaStarterModel");

            migrationBuilder.AddColumn<int>(
                name: "StarterId",
                table: "Pizzas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Pizzas",
                type: "decimal(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Pizzas_StarterId",
                table: "Pizzas",
                column: "StarterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pizzas_Starters_StarterId",
                table: "Pizzas",
                column: "StarterId",
                principalTable: "Starters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
