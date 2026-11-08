using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class RestrictTamanhoMarmita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_marmitas_tamanhosmarmita_tamanhoid",
                table: "marmitas");

            migrationBuilder.AddForeignKey(
                name: "FK_marmitas_tamanhosmarmita_tamanhoid",
                table: "marmitas",
                column: "tamanhoid",
                principalTable: "tamanhosmarmita",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_marmitas_tamanhosmarmita_tamanhoid",
                table: "marmitas");

            migrationBuilder.AddForeignKey(
                name: "FK_marmitas_tamanhosmarmita_tamanhoid",
                table: "marmitas",
                column: "tamanhoid",
                principalTable: "tamanhosmarmita",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
