using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class CreateEstadoPropostaTableAndLinkToProposta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Proposta");

            migrationBuilder.AddColumn<int>(
                name: "EstadoPropostaId",
                table: "Proposta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstadoProposta",
                columns: table => new
                {
                    EstadoPropostaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoProposta", x => x.EstadoPropostaId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_EstadoPropostaId",
                table: "Proposta",
                column: "EstadoPropostaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposta_EstadoProposta_EstadoPropostaId",
                table: "Proposta",
                column: "EstadoPropostaId",
                principalTable: "EstadoProposta",
                principalColumn: "EstadoPropostaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposta_EstadoProposta_EstadoPropostaId",
                table: "Proposta");

            migrationBuilder.DropTable(
                name: "EstadoProposta");

            migrationBuilder.DropIndex(
                name: "IX_Proposta_EstadoPropostaId",
                table: "Proposta");

            migrationBuilder.DropColumn(
                name: "EstadoPropostaId",
                table: "Proposta");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Proposta",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
