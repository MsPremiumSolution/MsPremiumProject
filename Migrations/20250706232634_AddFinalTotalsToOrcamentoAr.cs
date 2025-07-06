using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalTotalsToOrcamentoAr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxaIva",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFinalComIva",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorIva",
                table: "OrcamentoAr",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxaIva",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "TotalFinalComIva",
                table: "OrcamentoAr");

            migrationBuilder.DropColumn(
                name: "ValorIva",
                table: "OrcamentoAr");
        }
    }
}
