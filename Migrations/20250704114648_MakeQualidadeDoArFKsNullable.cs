using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class MakeQualidadeDoArFKsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "OrcamentoArId",
                table: "QualidadeDoAr",
                type: "bigint unsigned",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");

            migrationBuilder.AlterColumn<ulong>(
                name: "ObjetivosId",
                table: "QualidadeDoAr",
                type: "bigint unsigned",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");

            migrationBuilder.AlterColumn<ulong>(
                name: "DadosGeraisId",
                table: "QualidadeDoAr",
                type: "bigint unsigned",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "OrcamentoArId",
                table: "QualidadeDoAr",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "ObjetivosId",
                table: "QualidadeDoAr",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "DadosGeraisId",
                table: "QualidadeDoAr",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned",
                oldNullable: true);
        }
    }
}
