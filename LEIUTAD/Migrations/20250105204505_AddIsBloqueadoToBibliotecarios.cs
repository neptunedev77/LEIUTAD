using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LEIUTAD.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBloqueadoToBibliotecarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBloqueado",
                table: "Bibliotecarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBloqueado",
                table: "Bibliotecarios");
        }
    }
}
