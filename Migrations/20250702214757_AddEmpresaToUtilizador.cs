using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaToUtilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "Utilizadores",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "Utilizadores");
        }
    }
}
