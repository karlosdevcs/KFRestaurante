using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KFRestaurante.Migrations
{
    /// <inheritdoc />
    public partial class WhatsAppDoRestaurante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "whatsapp",
                table: "restaurantes",
                type: "text",
                nullable: true);

            // regra nova: so abre com WhatsApp. Quem ja estava aberto sem numero fecha ate o dono cadastrar
            migrationBuilder.Sql("UPDATE restaurantes SET aberto = false WHERE whatsapp IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "whatsapp",
                table: "restaurantes");
        }
    }
}
