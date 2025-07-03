using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class AddTipologiaConstrutivaTableAndRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "TipologiaConstrutivaId",
                table: "Proposta",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "TipologiasConstrutivas",
                columns: table => new
                {
                    TipologiaConstrutivaId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImagemUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipologiasConstrutivas", x => x.TipologiaConstrutivaId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_TipologiaConstrutivaId",
                table: "Proposta",
                column: "TipologiaConstrutivaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposta_TipologiasConstrutivas_TipologiaConstrutivaId",
                table: "Proposta",
                column: "TipologiaConstrutivaId",
                principalTable: "TipologiasConstrutivas",
                principalColumn: "TipologiaConstrutivaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposta_TipologiasConstrutivas_TipologiaConstrutivaId",
                table: "Proposta");

            migrationBuilder.DropTable(
                name: "TipologiasConstrutivas");

            migrationBuilder.DropIndex(
                name: "IX_Proposta_TipologiaConstrutivaId",
                table: "Proposta");

            migrationBuilder.DropColumn(
                name: "TipologiaConstrutivaId",
                table: "Proposta");
        }
    }
}
