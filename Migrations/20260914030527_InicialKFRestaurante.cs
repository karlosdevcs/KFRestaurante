using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class InicialKFRestaurante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardapiosDoDia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataPublicacao = table.Column<DateOnly>(type: "date", nullable: false),
                    Situacao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardapiosDoDia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Opcoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Esgotado = table.Column<bool>(type: "boolean", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opcoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Aberto = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TamanhosMarmita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    LimitedeComponente = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TamanhosMarmita", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    HoraPedido = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FormaDePagamento = table.Column<int>(type: "integer", nullable: false),
                    TipoDeEntrega = table.Column<int>(type: "integer", nullable: false),
                    CardapioDoDiaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedidos_CardapiosDoDia_CardapioDoDiaId",
                        column: x => x.CardapioDoDiaId,
                        principalTable: "CardapiosDoDia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardapioDoDiaOpcao",
                columns: table => new
                {
                    CardapioDoDiaId = table.Column<int>(type: "integer", nullable: false),
                    OpcoesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardapioDoDiaOpcao", x => new { x.CardapioDoDiaId, x.OpcoesId });
                    table.ForeignKey(
                        name: "FK_CardapioDoDiaOpcao_CardapiosDoDia_CardapioDoDiaId",
                        column: x => x.CardapioDoDiaId,
                        principalTable: "CardapiosDoDia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardapioDoDiaOpcao_Opcoes_OpcoesId",
                        column: x => x.OpcoesId,
                        principalTable: "Opcoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Marmitas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TamanhoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marmitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marmitas_TamanhosMarmita_TamanhoId",
                        column: x => x.TamanhoId,
                        principalTable: "TamanhosMarmita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemDoPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    PedidoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDoPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDoPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MarmitaOpcao",
                columns: table => new
                {
                    MarmitaId = table.Column<int>(type: "integer", nullable: false),
                    OpcoesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarmitaOpcao", x => new { x.MarmitaId, x.OpcoesId });
                    table.ForeignKey(
                        name: "FK_MarmitaOpcao_Marmitas_MarmitaId",
                        column: x => x.MarmitaId,
                        principalTable: "Marmitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarmitaOpcao_Opcoes_OpcoesId",
                        column: x => x.OpcoesId,
                        principalTable: "Opcoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardapioDoDiaOpcao_OpcoesId",
                table: "CardapioDoDiaOpcao",
                column: "OpcoesId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDoPedido_PedidoId",
                table: "ItemDoPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_MarmitaOpcao_OpcoesId",
                table: "MarmitaOpcao",
                column: "OpcoesId");

            migrationBuilder.CreateIndex(
                name: "IX_Marmitas_TamanhoId",
                table: "Marmitas",
                column: "TamanhoId");

            migrationBuilder.CreateIndex(
                name: "IX_Opcoes_Nome",
                table: "Opcoes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_CardapioDoDiaId",
                table: "Pedidos",
                column: "CardapioDoDiaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_Numero",
                table: "Pedidos",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardapioDoDiaOpcao");

            migrationBuilder.DropTable(
                name: "ItemDoPedido");

            migrationBuilder.DropTable(
                name: "MarmitaOpcao");

            migrationBuilder.DropTable(
                name: "Restaurantes");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Marmitas");

            migrationBuilder.DropTable(
                name: "Opcoes");

            migrationBuilder.DropTable(
                name: "CardapiosDoDia");

            migrationBuilder.DropTable(
                name: "TamanhosMarmita");
        }
    }
}
