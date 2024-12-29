using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LEIUTAD.Migrations
{
    public partial class AtualizarEmp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emprestimo",
                columns: table => new
                {
                    ID_Emp = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ID_Leitor = table.Column<int>(type: "int", nullable: false),
                    Data_Req = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Data_Dev = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprestimo", x => x.ID_Emp);
                    table.ForeignKey(
                        name: "FK_Emprestimo_Leitor_ID_Leitor",
                        column: x => x.ID_Leitor,
                        principalTable: "Leitor",
                        principalColumn: "ID_user",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Emprestimo_Livro",
                columns: table => new
                {
                    ID_Emp = table.Column<int>(type: "int", nullable: false),
                    ISBN = table.Column<string>(type: "varchar(13)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprestimo_Livro", x => new { x.ID_Emp, x.ISBN });
                    table.ForeignKey(
                        name: "FK_Emprestimo_Livro_Emprestimo_ID_Emp",
                        column: x => x.ID_Emp,
                        principalTable: "Emprestimo",
                        principalColumn: "ID_Emp",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emprestimo_Livro_Livro_ISBN",
                        column: x => x.ISBN,
                        principalTable: "Livro",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimo_ID_Leitor",
                table: "Emprestimo",
                column: "ID_Leitor");

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimo_Livro_ISBN",
                table: "Emprestimo_Livro",
                column: "ISBN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emprestimo_Livro");

            migrationBuilder.DropTable(
                name: "Emprestimo");
        }
    }
}
