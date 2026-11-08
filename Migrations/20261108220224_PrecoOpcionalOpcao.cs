using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class PrecoOpcionalOpcao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "preco",
                table: "opcoes",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            // o 0 era so o valor padrao do codigo antigo, nao um preco escolhido: vira "sem preco" (so componente)
            migrationBuilder.Sql("UPDATE opcoes SET preco = NULL WHERE preco = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // antes de voltar a NOT NULL, quem esta sem preco volta a ter 0 (senao o ALTER falha)
            migrationBuilder.Sql("UPDATE opcoes SET preco = 0 WHERE preco IS NULL;");

            migrationBuilder.AlterColumn<decimal>(
                name: "preco",
                table: "opcoes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
