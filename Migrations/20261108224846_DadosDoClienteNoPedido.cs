using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class DadosDoClienteNoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // pedidos que ja existem nao tem nome: recebem "Não informado"
            migrationBuilder.AddColumn<string>(
                name: "nomecliente",
                table: "pedidos",
                type: "text",
                nullable: false,
                defaultValue: "Não informado");

            // o default servia so para os pedidos antigos; daqui em diante o dominio sempre informa o nome
            migrationBuilder.AlterColumn<string>(
                name: "nomecliente",
                table: "pedidos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Não informado");

            migrationBuilder.AddColumn<string>(
                name: "observacao",
                table: "pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "referencia",
                table: "pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "trocopara",
                table: "pedidos",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nomecliente",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "observacao",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "referencia",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "trocopara",
                table: "pedidos");
        }
    }
}
