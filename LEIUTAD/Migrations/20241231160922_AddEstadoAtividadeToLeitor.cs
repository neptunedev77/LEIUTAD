using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LEIUTAD.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoAtividadeToLeitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Atividade",
                table: "Leitor",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Atividade",
                table: "Leitor");
        }
    }
}
