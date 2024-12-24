using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LEIUTAD.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemAndSinopseToLivros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Livro",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Sinopse",
                table: "Livro",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "Sinopse",
                table: "Livro");
        }
    }
}
