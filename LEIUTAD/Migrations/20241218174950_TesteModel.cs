using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LEIUTAD.Migrations
{
    /// <inheritdoc />
    public partial class TesteModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livro_Autor_AutorID_Autor",
                table: "Livro");

            migrationBuilder.DropForeignKey(
                name: "FK_Livro_Genero_GeneroID_Genero",
                table: "Livro");

            migrationBuilder.DropIndex(
                name: "IX_Livro_AutorID_Autor",
                table: "Livro");

            migrationBuilder.DropIndex(
                name: "IX_Livro_GeneroID_Genero",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "AutorID_Autor",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "GeneroID_Genero",
                table: "Livro");

            migrationBuilder.CreateIndex(
                name: "IX_Livro_ID_Autor",
                table: "Livro",
                column: "ID_Autor");

            migrationBuilder.CreateIndex(
                name: "IX_Livro_ID_Genero",
                table: "Livro",
                column: "ID_Genero");

            migrationBuilder.AddForeignKey(
                name: "FK_Livro_Autor_ID_Autor",
                table: "Livro",
                column: "ID_Autor",
                principalTable: "Autor",
                principalColumn: "ID_Autor",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Livro_Genero_ID_Genero",
                table: "Livro",
                column: "ID_Genero",
                principalTable: "Genero",
                principalColumn: "ID_Genero",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livro_Autor_ID_Autor",
                table: "Livro");

            migrationBuilder.DropForeignKey(
                name: "FK_Livro_Genero_ID_Genero",
                table: "Livro");

            migrationBuilder.DropIndex(
                name: "IX_Livro_ID_Autor",
                table: "Livro");

            migrationBuilder.DropIndex(
                name: "IX_Livro_ID_Genero",
                table: "Livro");

            migrationBuilder.AddColumn<int>(
                name: "AutorID_Autor",
                table: "Livro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GeneroID_Genero",
                table: "Livro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Livro_AutorID_Autor",
                table: "Livro",
                column: "AutorID_Autor");

            migrationBuilder.CreateIndex(
                name: "IX_Livro_GeneroID_Genero",
                table: "Livro",
                column: "GeneroID_Genero");

            migrationBuilder.AddForeignKey(
                name: "FK_Livro_Autor_AutorID_Autor",
                table: "Livro",
                column: "AutorID_Autor",
                principalTable: "Autor",
                principalColumn: "ID_Autor",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Livro_Genero_GeneroID_Genero",
                table: "Livro",
                column: "GeneroID_Genero",
                principalTable: "Genero",
                principalColumn: "ID_Genero",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
