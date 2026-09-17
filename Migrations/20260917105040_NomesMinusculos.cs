using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class NomesMinusculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardapioDoDiaOpcao_CardapiosDoDia_CardapioDoDiaId",
                table: "CardapioDoDiaOpcao");

            migrationBuilder.DropForeignKey(
                name: "FK_CardapioDoDiaOpcao_Opcoes_OpcoesId",
                table: "CardapioDoDiaOpcao");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDoPedido_Pedidos_PedidoId",
                table: "ItemDoPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_MarmitaOpcao_Marmitas_MarmitaId",
                table: "MarmitaOpcao");

            migrationBuilder.DropForeignKey(
                name: "FK_MarmitaOpcao_Opcoes_OpcoesId",
                table: "MarmitaOpcao");

            migrationBuilder.DropForeignKey(
                name: "FK_Marmitas_TamanhosMarmita_TamanhoId",
                table: "Marmitas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_CardapiosDoDia_CardapioDoDiaId",
                table: "Pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TamanhosMarmita",
                table: "TamanhosMarmita");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Restaurantes",
                table: "Restaurantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pedidos",
                table: "Pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Opcoes",
                table: "Opcoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Marmitas",
                table: "Marmitas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarmitaOpcao",
                table: "MarmitaOpcao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemDoPedido",
                table: "ItemDoPedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardapiosDoDia",
                table: "CardapiosDoDia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardapioDoDiaOpcao",
                table: "CardapioDoDiaOpcao");

            migrationBuilder.RenameTable(
                name: "TamanhosMarmita",
                newName: "tamanhosmarmita");

            migrationBuilder.RenameTable(
                name: "Restaurantes",
                newName: "restaurantes");

            migrationBuilder.RenameTable(
                name: "Pedidos",
                newName: "pedidos");

            migrationBuilder.RenameTable(
                name: "Opcoes",
                newName: "opcoes");

            migrationBuilder.RenameTable(
                name: "Marmitas",
                newName: "marmitas");

            migrationBuilder.RenameTable(
                name: "MarmitaOpcao",
                newName: "marmitaopcao");

            migrationBuilder.RenameTable(
                name: "ItemDoPedido",
                newName: "itemdopedido");

            migrationBuilder.RenameTable(
                name: "CardapiosDoDia",
                newName: "cardapiosdodia");

            migrationBuilder.RenameTable(
                name: "CardapioDoDiaOpcao",
                newName: "cardapiododiaopcao");

            migrationBuilder.RenameColumn(
                name: "Preco",
                table: "tamanhosmarmita",
                newName: "preco");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "tamanhosmarmita",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "LimitedeComponente",
                table: "tamanhosmarmita",
                newName: "limitedecomponente");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tamanhosmarmita",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Aberto",
                table: "restaurantes",
                newName: "aberto");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "restaurantes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoDeEntrega",
                table: "pedidos",
                newName: "tipodeentrega");

            migrationBuilder.RenameColumn(
                name: "Numero",
                table: "pedidos",
                newName: "numero");

            migrationBuilder.RenameColumn(
                name: "HoraPedido",
                table: "pedidos",
                newName: "horapedido");

            migrationBuilder.RenameColumn(
                name: "FormaDePagamento",
                table: "pedidos",
                newName: "formadepagamento");

            migrationBuilder.RenameColumn(
                name: "CardapioDoDiaId",
                table: "pedidos",
                newName: "cardapiododiaid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pedidos",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_Numero",
                table: "pedidos",
                newName: "IX_pedidos_numero");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_CardapioDoDiaId",
                table: "pedidos",
                newName: "IX_pedidos_cardapiododiaid");

            migrationBuilder.RenameColumn(
                name: "Preco",
                table: "opcoes",
                newName: "preco");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "opcoes",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Esgotado",
                table: "opcoes",
                newName: "esgotado");

            migrationBuilder.RenameColumn(
                name: "Categoria",
                table: "opcoes",
                newName: "categoria");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "opcoes",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Opcoes_Nome",
                table: "opcoes",
                newName: "IX_opcoes_nome");

            migrationBuilder.RenameColumn(
                name: "TamanhoId",
                table: "marmitas",
                newName: "tamanhoid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "marmitas",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Marmitas_TamanhoId",
                table: "marmitas",
                newName: "IX_marmitas_tamanhoid");

            migrationBuilder.RenameColumn(
                name: "OpcoesId",
                table: "marmitaopcao",
                newName: "opcoesid");

            migrationBuilder.RenameColumn(
                name: "MarmitaId",
                table: "marmitaopcao",
                newName: "marmitaid");

            migrationBuilder.RenameIndex(
                name: "IX_MarmitaOpcao_OpcoesId",
                table: "marmitaopcao",
                newName: "IX_marmitaopcao_opcoesid");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "itemdopedido",
                newName: "quantidade");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "itemdopedido",
                newName: "precounitario");

            migrationBuilder.RenameColumn(
                name: "PedidoId",
                table: "itemdopedido",
                newName: "pedidoid");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "itemdopedido",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "itemdopedido",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_ItemDoPedido_PedidoId",
                table: "itemdopedido",
                newName: "IX_itemdopedido_pedidoid");

            migrationBuilder.RenameColumn(
                name: "Situacao",
                table: "cardapiosdodia",
                newName: "situacao");

            migrationBuilder.RenameColumn(
                name: "DataPublicacao",
                table: "cardapiosdodia",
                newName: "datapublicacao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "cardapiosdodia",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OpcoesId",
                table: "cardapiododiaopcao",
                newName: "opcoesid");

            migrationBuilder.RenameColumn(
                name: "CardapioDoDiaId",
                table: "cardapiododiaopcao",
                newName: "cardapiododiaid");

            migrationBuilder.RenameIndex(
                name: "IX_CardapioDoDiaOpcao_OpcoesId",
                table: "cardapiododiaopcao",
                newName: "IX_cardapiododiaopcao_opcoesid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tamanhosmarmita",
                table: "tamanhosmarmita",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_restaurantes",
                table: "restaurantes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pedidos",
                table: "pedidos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_opcoes",
                table: "opcoes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_marmitas",
                table: "marmitas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_marmitaopcao",
                table: "marmitaopcao",
                columns: new[] { "marmitaid", "opcoesid" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_itemdopedido",
                table: "itemdopedido",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cardapiosdodia",
                table: "cardapiosdodia",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cardapiododiaopcao",
                table: "cardapiododiaopcao",
                columns: new[] { "cardapiododiaid", "opcoesid" });

            migrationBuilder.AddForeignKey(
                name: "FK_cardapiododiaopcao_cardapiosdodia_cardapiododiaid",
                table: "cardapiododiaopcao",
                column: "cardapiododiaid",
                principalTable: "cardapiosdodia",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cardapiododiaopcao_opcoes_opcoesid",
                table: "cardapiododiaopcao",
                column: "opcoesid",
                principalTable: "opcoes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido",
                column: "pedidoid",
                principalTable: "pedidos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_marmitaopcao_marmitas_marmitaid",
                table: "marmitaopcao",
                column: "marmitaid",
                principalTable: "marmitas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_marmitaopcao_opcoes_opcoesid",
                table: "marmitaopcao",
                column: "opcoesid",
                principalTable: "opcoes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_marmitas_tamanhosmarmita_tamanhoid",
                table: "marmitas",
                column: "tamanhoid",
                principalTable: "tamanhosmarmita",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_pedidos_cardapiosdodia_cardapiododiaid",
                table: "pedidos",
                column: "cardapiododiaid",
                principalTable: "cardapiosdodia",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cardapiododiaopcao_cardapiosdodia_cardapiododiaid",
                table: "cardapiododiaopcao");

            migrationBuilder.DropForeignKey(
                name: "FK_cardapiododiaopcao_opcoes_opcoesid",
                table: "cardapiododiaopcao");

            migrationBuilder.DropForeignKey(
                name: "FK_itemdopedido_pedidos_pedidoid",
                table: "itemdopedido");

            migrationBuilder.DropForeignKey(
                name: "FK_marmitaopcao_marmitas_marmitaid",
                table: "marmitaopcao");

            migrationBuilder.DropForeignKey(
                name: "FK_marmitaopcao_opcoes_opcoesid",
                table: "marmitaopcao");

            migrationBuilder.DropForeignKey(
                name: "FK_marmitas_tamanhosmarmita_tamanhoid",
                table: "marmitas");

            migrationBuilder.DropForeignKey(
                name: "FK_pedidos_cardapiosdodia_cardapiododiaid",
                table: "pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tamanhosmarmita",
                table: "tamanhosmarmita");

            migrationBuilder.DropPrimaryKey(
                name: "PK_restaurantes",
                table: "restaurantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pedidos",
                table: "pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_opcoes",
                table: "opcoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_marmitas",
                table: "marmitas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_marmitaopcao",
                table: "marmitaopcao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_itemdopedido",
                table: "itemdopedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cardapiosdodia",
                table: "cardapiosdodia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cardapiododiaopcao",
                table: "cardapiododiaopcao");

            migrationBuilder.RenameTable(
                name: "tamanhosmarmita",
                newName: "TamanhosMarmita");

            migrationBuilder.RenameTable(
                name: "restaurantes",
                newName: "Restaurantes");

            migrationBuilder.RenameTable(
                name: "pedidos",
                newName: "Pedidos");

            migrationBuilder.RenameTable(
                name: "opcoes",
                newName: "Opcoes");

            migrationBuilder.RenameTable(
                name: "marmitas",
                newName: "Marmitas");

            migrationBuilder.RenameTable(
                name: "marmitaopcao",
                newName: "MarmitaOpcao");

            migrationBuilder.RenameTable(
                name: "itemdopedido",
                newName: "ItemDoPedido");

            migrationBuilder.RenameTable(
                name: "cardapiosdodia",
                newName: "CardapiosDoDia");

            migrationBuilder.RenameTable(
                name: "cardapiododiaopcao",
                newName: "CardapioDoDiaOpcao");

            migrationBuilder.RenameColumn(
                name: "preco",
                table: "TamanhosMarmita",
                newName: "Preco");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "TamanhosMarmita",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "limitedecomponente",
                table: "TamanhosMarmita",
                newName: "LimitedeComponente");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TamanhosMarmita",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "aberto",
                table: "Restaurantes",
                newName: "Aberto");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Restaurantes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipodeentrega",
                table: "Pedidos",
                newName: "TipoDeEntrega");

            migrationBuilder.RenameColumn(
                name: "numero",
                table: "Pedidos",
                newName: "Numero");

            migrationBuilder.RenameColumn(
                name: "horapedido",
                table: "Pedidos",
                newName: "HoraPedido");

            migrationBuilder.RenameColumn(
                name: "formadepagamento",
                table: "Pedidos",
                newName: "FormaDePagamento");

            migrationBuilder.RenameColumn(
                name: "cardapiododiaid",
                table: "Pedidos",
                newName: "CardapioDoDiaId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Pedidos",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_pedidos_numero",
                table: "Pedidos",
                newName: "IX_Pedidos_Numero");

            migrationBuilder.RenameIndex(
                name: "IX_pedidos_cardapiododiaid",
                table: "Pedidos",
                newName: "IX_Pedidos_CardapioDoDiaId");

            migrationBuilder.RenameColumn(
                name: "preco",
                table: "Opcoes",
                newName: "Preco");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Opcoes",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "esgotado",
                table: "Opcoes",
                newName: "Esgotado");

            migrationBuilder.RenameColumn(
                name: "categoria",
                table: "Opcoes",
                newName: "Categoria");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Opcoes",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_opcoes_nome",
                table: "Opcoes",
                newName: "IX_Opcoes_Nome");

            migrationBuilder.RenameColumn(
                name: "tamanhoid",
                table: "Marmitas",
                newName: "TamanhoId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Marmitas",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_marmitas_tamanhoid",
                table: "Marmitas",
                newName: "IX_Marmitas_TamanhoId");

            migrationBuilder.RenameColumn(
                name: "opcoesid",
                table: "MarmitaOpcao",
                newName: "OpcoesId");

            migrationBuilder.RenameColumn(
                name: "marmitaid",
                table: "MarmitaOpcao",
                newName: "MarmitaId");

            migrationBuilder.RenameIndex(
                name: "IX_marmitaopcao_opcoesid",
                table: "MarmitaOpcao",
                newName: "IX_MarmitaOpcao_OpcoesId");

            migrationBuilder.RenameColumn(
                name: "quantidade",
                table: "ItemDoPedido",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "precounitario",
                table: "ItemDoPedido",
                newName: "PrecoUnitario");

            migrationBuilder.RenameColumn(
                name: "pedidoid",
                table: "ItemDoPedido",
                newName: "PedidoId");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "ItemDoPedido",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ItemDoPedido",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_itemdopedido_pedidoid",
                table: "ItemDoPedido",
                newName: "IX_ItemDoPedido_PedidoId");

            migrationBuilder.RenameColumn(
                name: "situacao",
                table: "CardapiosDoDia",
                newName: "Situacao");

            migrationBuilder.RenameColumn(
                name: "datapublicacao",
                table: "CardapiosDoDia",
                newName: "DataPublicacao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CardapiosDoDia",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "opcoesid",
                table: "CardapioDoDiaOpcao",
                newName: "OpcoesId");

            migrationBuilder.RenameColumn(
                name: "cardapiododiaid",
                table: "CardapioDoDiaOpcao",
                newName: "CardapioDoDiaId");

            migrationBuilder.RenameIndex(
                name: "IX_cardapiododiaopcao_opcoesid",
                table: "CardapioDoDiaOpcao",
                newName: "IX_CardapioDoDiaOpcao_OpcoesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TamanhosMarmita",
                table: "TamanhosMarmita",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Restaurantes",
                table: "Restaurantes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pedidos",
                table: "Pedidos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Opcoes",
                table: "Opcoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Marmitas",
                table: "Marmitas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarmitaOpcao",
                table: "MarmitaOpcao",
                columns: new[] { "MarmitaId", "OpcoesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemDoPedido",
                table: "ItemDoPedido",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardapiosDoDia",
                table: "CardapiosDoDia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardapioDoDiaOpcao",
                table: "CardapioDoDiaOpcao",
                columns: new[] { "CardapioDoDiaId", "OpcoesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CardapioDoDiaOpcao_CardapiosDoDia_CardapioDoDiaId",
                table: "CardapioDoDiaOpcao",
                column: "CardapioDoDiaId",
                principalTable: "CardapiosDoDia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CardapioDoDiaOpcao_Opcoes_OpcoesId",
                table: "CardapioDoDiaOpcao",
                column: "OpcoesId",
                principalTable: "Opcoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDoPedido_Pedidos_PedidoId",
                table: "ItemDoPedido",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarmitaOpcao_Marmitas_MarmitaId",
                table: "MarmitaOpcao",
                column: "MarmitaId",
                principalTable: "Marmitas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarmitaOpcao_Opcoes_OpcoesId",
                table: "MarmitaOpcao",
                column: "OpcoesId",
                principalTable: "Opcoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marmitas_TamanhosMarmita_TamanhoId",
                table: "Marmitas",
                column: "TamanhoId",
                principalTable: "TamanhosMarmita",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_CardapiosDoDia_CardapioDoDiaId",
                table: "Pedidos",
                column: "CardapioDoDiaId",
                principalTable: "CardapiosDoDia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
