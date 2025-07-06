using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class RestructureOrcamentoArWithLinhas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAcessoriosExtras",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "HasAdaptacaoSistema",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "HasControleTecnico",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "HasExecucaoProjeto",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "HasInstalacaoMaoDeObra",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "HasVigilancia24h",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorConfiguracaoFabricacao",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorFabricacao",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorImplementacaoTrabalho",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorManutencao",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorPersonalizacao",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorProjeto",
                table: "OrcamentoAr");

            migrationBuilder.CreateTable(
                name: "OrcamentoArLinhas",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrcamentoArId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CodigoItem = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLinha = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoArLinhas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrcamentoArLinhas_OrcamentoAr_OrcamentoArId",
                        column: x => x.OrcamentoArId,
                        principalTable: "OrcamentoAr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoArLinhas_OrcamentoArId",
                table: "OrcamentoArLinhas",
                column: "OrcamentoArId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrcamentoArLinhas");

            migrationBuilder.AddColumn<bool>(
                name: "HasAcessoriosExtras",
                table: "OrcamentoAr",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasAdaptacaoSistema",
                table: "OrcamentoAr",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasControleTecnico",
                table: "OrcamentoAr",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasExecucaoProjeto",
                table: "OrcamentoAr",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasInstalacaoMaoDeObra",
                table: "OrcamentoAr",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasVigilancia24h",
                table: "OrcamentoAr",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorConfiguracaoFabricacao",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFabricacao",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorImplementacaoTrabalho",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorManutencao",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPersonalizacao",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorProjeto",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
