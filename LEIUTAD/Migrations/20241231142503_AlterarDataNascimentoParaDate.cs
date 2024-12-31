using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LEIUTAD.Migrations
{
    /// <inheritdoc />
    public partial class AlterarDataNascimentoParaDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Data_n",
                table: "Leitor",
                type: "date", // Alterar para "date"
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Data_n",
                table: "Leitor",
                type: "datetime2", // Reverta para "datetime2" no Down()
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }

    }
}
