using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class RestrictCardapioPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedidos_cardapiosdodia_cardapiododiaid",
                table: "pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_pedidos_cardapiosdodia_cardapiododiaid",
                table: "pedidos",
                column: "cardapiododiaid",
                principalTable: "cardapiosdodia",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedidos_cardapiosdodia_cardapiododiaid",
                table: "pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_pedidos_cardapiosdodia_cardapiododiaid",
                table: "pedidos",
                column: "cardapiododiaid",
                principalTable: "cardapiosdodia",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
