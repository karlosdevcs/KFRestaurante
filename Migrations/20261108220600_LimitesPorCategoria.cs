using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class LimitesPorCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "limitescategoria",
                columns: table => new
                {
                    categoria = table.Column<int>(type: "integer", nullable: false),
                    tamanhomarmitaid = table.Column<int>(type: "integer", nullable: false),
                    maximo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_limitescategoria", x => new { x.tamanhomarmitaid, x.categoria });
                    table.ForeignKey(
                        name: "FK_limitescategoria_tamanhosmarmita_tamanhomarmitaid",
                        column: x => x.tamanhomarmitaid,
                        principalTable: "tamanhosmarmita",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "limitescategoria");
        }
    }
}
