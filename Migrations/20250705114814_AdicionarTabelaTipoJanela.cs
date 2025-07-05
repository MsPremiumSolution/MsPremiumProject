using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaTipoJanela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "TipoJanelaId",
                table: "DadosGerais",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tipojanelas",
                columns: table => new
                {
                    TipoJanelaId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TipoJanela1 = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipojanelas", x => x.TipoJanelaId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_DadosGerais_TipoJanelaId",
                table: "DadosGerais",
                column: "TipoJanelaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DadosGerais_Tipojanelas_TipoJanelaId",
                table: "DadosGerais",
                column: "TipoJanelaId",
                principalTable: "Tipojanelas",
                principalColumn: "TipoJanelaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DadosGerais_Tipojanelas_TipoJanelaId",
                table: "DadosGerais");

            migrationBuilder.DropTable(
                name: "Tipojanelas");

            migrationBuilder.DropIndex(
                name: "IX_DadosGerais_TipoJanelaId",
                table: "DadosGerais");

            migrationBuilder.DropColumn(
                name: "TipoJanelaId",
                table: "DadosGerais");
        }
    }
}
