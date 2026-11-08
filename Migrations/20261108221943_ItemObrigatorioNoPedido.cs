using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class ItemObrigatorioNoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido");

            migrationBuilder.AlterColumn<int>(
                name: "pedidoid",
                table: "itemdopedido",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido",
                column: "pedidoid",
                principalTable: "pedidos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido");

            migrationBuilder.AlterColumn<int>(
                name: "pedidoid",
                table: "itemdopedido",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido",
                column: "pedidoid",
                principalTable: "pedidos",
                principalColumn: "id");
        }
    }
}
