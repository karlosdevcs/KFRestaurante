using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class TelefoneDoCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // pedidos que ja existem nao tem telefone: ficam com '' (= nao informado).
            // Nao apagamos pedidos aqui: esta migration tambem roda em producao, onde eles seriam vendas reais
            migrationBuilder.AddColumn<string>(
                name: "telefone",
                table: "pedidos",
                type: "text",
                nullable: false,
                defaultValue: "");

            // o default servia so para os antigos; daqui em diante o dominio sempre informa o telefone
            migrationBuilder.AlterColumn<string>(
                name: "telefone",
                table: "pedidos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "telefone",
                table: "pedidos");
        }
    }
}
