using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class AtualizadoEmDoCardapio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "atualizadoem",
                table: "cardapiosdodia",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()"); // cardapios que ja existem ganham a hora da migration em vez de 0001-01-01
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "atualizadoem",
                table: "cardapiosdodia");
        }
    }
}
