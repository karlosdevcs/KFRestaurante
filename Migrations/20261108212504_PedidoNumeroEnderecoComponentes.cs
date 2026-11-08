using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class PedidoNumeroEnderecoComponentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "pedidos_numero_seq");

            migrationBuilder.AlterColumn<int>(
                name: "numero",
                table: "pedidos",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('pedidos_numero_seq')",
                oldClrType: typeof(int),
                oldType: "integer");

            // se ja existirem pedidos, a sequence continua depois do maior numero (senao comeca em 1)
            migrationBuilder.Sql("SELECT setval('pedidos_numero_seq', COALESCE((SELECT MAX(numero) FROM pedidos), 0) + 1, false);");

            migrationBuilder.AddColumn<string>(
                name: "endereco",
                table: "pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "componentes",
                table: "itemdopedido",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endereco",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "componentes",
                table: "itemdopedido");

            // primeiro tira o default que usa a sequence, depois apaga a sequence (o Postgres nao deixa na ordem inversa)
            migrationBuilder.AlterColumn<int>(
                name: "numero",
                table: "pedidos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValueSql: "nextval('pedidos_numero_seq')");

            migrationBuilder.DropSequence(
                name: "pedidos_numero_seq");
        }
    }
}
