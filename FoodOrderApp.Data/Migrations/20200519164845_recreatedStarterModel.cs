using Microsoft.EntityFrameworkCore.Migrations;

namespace FoodOrderApp.Data.Migrations
{
    public partial class recreatedStarterModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StarterPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Starters",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Starters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Starters");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Starters");

            migrationBuilder.CreateTable(
                name: "StarterPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    StarterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarterPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StarterPrices_Starters_StarterId",
                        column: x => x.StarterId,
                        principalTable: "Starters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StarterPrices_StarterId",
                table: "StarterPrices",
                column: "StarterId");
        }
    }
}
