using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaAndNomeLocalidadeTextoToCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Localidade",
                table: "Clientes");

            migrationBuilder.AlterColumn<string>(
                name: "Empresa",
                table: "Clientes",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AddColumn<string>(
                name: "NomeLocalidadeTexto",
                table: "Clientes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeLocalidadeTexto",
                table: "Clientes");

            migrationBuilder.AlterColumn<string>(
                name: "Empresa",
                table: "Clientes",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AddColumn<string>(
                name: "Localidade",
                table: "Clientes",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
