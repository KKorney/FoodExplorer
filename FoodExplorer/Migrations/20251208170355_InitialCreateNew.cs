using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodExplorer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    BarCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ImageFrontSmallUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Ingredients = table.Column<string>(type: "TEXT", nullable: false),
                    NutriScore = table.Column<string>(type: "TEXT", nullable: false),
                    Additives = table.Column<string>(type: "TEXT", nullable: false),
                    Traces = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdatedDate = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.BarCode);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    ProductBarCode = table.Column<string>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    AddedDate = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.ProductBarCode);
                    table.ForeignKey(
                        name: "FK_Favorites_Products_ProductBarCode",
                        column: x => x.ProductBarCode,
                        principalTable: "Products",
                        principalColumn: "BarCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConsultationDate = table.Column<long>(type: "INTEGER", nullable: false),
                    ProductBarCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryEntries_Products_ProductBarCode",
                        column: x => x.ProductBarCode,
                        principalTable: "Products",
                        principalColumn: "BarCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutrientLevels",
                columns: table => new
                {
                    ProductBarCode = table.Column<string>(type: "TEXT", nullable: false),
                    Fat = table.Column<string>(type: "TEXT", nullable: false),
                    SaturatedFat = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<string>(type: "TEXT", nullable: false),
                    Sugars = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutrientLevels", x => x.ProductBarCode);
                    table.ForeignKey(
                        name: "FK_NutrientLevels_Products_ProductBarCode",
                        column: x => x.ProductBarCode,
                        principalTable: "Products",
                        principalColumn: "BarCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoryEntries_ProductBarCode",
                table: "HistoryEntries",
                column: "ProductBarCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "HistoryEntries");

            migrationBuilder.DropTable(
                name: "NutrientLevels");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
