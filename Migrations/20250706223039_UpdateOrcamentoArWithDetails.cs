using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrcamentoArWithDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfiguracaoFabricacao",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ImplementacaoTrabalho",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "Manutencao",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "Personalizacao",
                table: "OrcamentoAr");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorProjeto",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorFabricacao",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<string>(
                name: "FiltroManutencao",
                table: "OrcamentoAr",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "M3VolumeHabitavel",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumeroCompartimentos",
                table: "OrcamentoAr",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumeroPisos",
                table: "OrcamentoAr",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorConfiguracaoFabricacao",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiltroManutencao",
                table: "OrcamentoAr");

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
                name: "M3VolumeHabitavel",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "NumeroCompartimentos",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "NumeroPisos",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorConfiguracaoFabricacao",
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

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorProjeto",
                table: "OrcamentoAr",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorFabricacao",
                table: "OrcamentoAr",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ConfiguracaoFabricacao",
                table: "OrcamentoAr",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImplementacaoTrabalho",
                table: "OrcamentoAr",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Manutencao",
                table: "OrcamentoAr",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Personalizacao",
                table: "OrcamentoAr",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
