using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class AddVolumesAndMedidasTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comprimento",
                table: "Volumes");

            migrationBuilder.DropColumn(
                name: "Largura",
                table: "Volumes");

            migrationBuilder.AlterColumn<decimal>(
                name: "Altura",
                table: "Volumes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.CreateTable(
                name: "Medidas",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VolumeId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Largura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comprimento = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medidas_Volumes_VolumeId",
                        column: x => x.VolumeId,
                        principalTable: "Volumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Medidas_VolumeId",
                table: "Medidas",
                column: "VolumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medidas");

            migrationBuilder.AlterColumn<decimal>(
                name: "Altura",
                table: "Volumes",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "Comprimento",
                table: "Volumes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Largura",
                table: "Volumes",
                type: "decimal(65,30)",
                nullable: true);
        }
    }
}
