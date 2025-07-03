using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class MakeTipologiaIdNullableInProposta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposta_TipologiasConstrutivas_TipologiaConstrutivaId",
                table: "Proposta");

            migrationBuilder.AlterColumn<ulong>(
                name: "TipologiaConstrutivaId",
                table: "Proposta",
                type: "bigint unsigned",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposta_TipologiasConstrutivas_TipologiaConstrutivaId",
                table: "Proposta",
                column: "TipologiaConstrutivaId",
                principalTable: "TipologiasConstrutivas",
                principalColumn: "TipologiaConstrutivaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposta_TipologiasConstrutivas_TipologiaConstrutivaId",
                table: "Proposta");

            migrationBuilder.AlterColumn<ulong>(
                name: "TipologiaConstrutivaId",
                table: "Proposta",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposta_TipologiasConstrutivas_TipologiaConstrutivaId",
                table: "Proposta",
                column: "TipologiaConstrutivaId",
                principalTable: "TipologiasConstrutivas",
                principalColumn: "TipologiaConstrutivaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
