using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class MakeTelefone2Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ADICIONE ESTA LINHA:
            // Isto diz à base de dados para alterar a coluna "Telefone2" na tabela "Clientes"
            // para permitir valores nulos (nullable: true).
            migrationBuilder.AlterColumn<long>(
                name: "Telefone2",
                table: "Clientes",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ADICIONE ESTA LINHA:
            // Isto desfaz a alteração, tornando a coluna obrigatória novamente
            // (nullable: false).
            migrationBuilder.AlterColumn<long>(
                name: "Telefone2",
                table: "Clientes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}