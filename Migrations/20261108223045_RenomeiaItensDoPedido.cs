using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class RenomeiaItensDoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_itemdopedido",
                table: "itemdopedido");

            migrationBuilder.RenameTable(
                name: "itemdopedido",
                newName: "itensdopedido");

            migrationBuilder.RenameIndex(
                name: "IX_itemdopedido_pedidoid",
                table: "itensdopedido",
                newName: "IX_itensdopedido_pedidoid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_itensdopedido",
                table: "itensdopedido",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_itensdopedido_pedidos_pedidoid",
                table: "itensdopedido",
                column: "pedidoid",
                principalTable: "pedidos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itensdopedido_pedidos_pedidoid",
                table: "itensdopedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_itensdopedido",
                table: "itensdopedido");

            migrationBuilder.RenameTable(
                name: "itensdopedido",
                newName: "itemdopedido");

            migrationBuilder.RenameIndex(
                name: "IX_itensdopedido_pedidoid",
                table: "itemdopedido",
                newName: "IX_itemdopedido_pedidoid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_itemdopedido",
                table: "itemdopedido",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido",
                column: "pedidoid",
                principalTable: "pedidos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
