using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStringLocalidadeFromCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calculovolumes_Volumes_VolumeId",
                table: "Calculovolumes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Localidades_LocalidadeId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Construtivos_Proposta_PropostaId",
                table: "Construtivos");

            migrationBuilder.DropForeignKey(
                name: "FK_Dadosgerals_Proposta_PropostaId",
                table: "Dadosgerals");

            migrationBuilder.DropForeignKey(
                name: "FK_Dadosgerals_Tipojanelas_DgTipoJanelaId",
                table: "Dadosgerals");

            migrationBuilder.DropForeignKey(
                name: "FK_Dadosgerals_Tipoobras_TipoObraId",
                table: "Dadosgerals");

            migrationBuilder.DropForeignKey(
                name: "FK_Dadosgerals_Tipotratamentos_TipoTratamentoId",
                table: "Dadosgerals");

            migrationBuilder.DropForeignKey(
                name: "FK_Higrometria_Dadosgerals_DadosGeralId",
                table: "Higrometria");

            migrationBuilder.DropForeignKey(
                name: "FK_Higrometria_Proposta_PropostaId",
                table: "Higrometria");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetToken_Utilizador_UtilizadorId",
                table: "PasswordResetToken");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposta_Clientes_ClienteId",
                table: "Proposta");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposta_Utilizador_UtilizadorId",
                table: "Proposta");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualidadears_Dadosgerals_DadosGeralId",
                table: "Qualidadears");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualidadears_Volumes_VolumeId",
                table: "Qualidadears");

            migrationBuilder.DropForeignKey(
                name: "FK_Sintomas_Dadosgerals_DadosGeralId",
                table: "Sintomas");

            migrationBuilder.DropForeignKey(
                name: "FK_Sintomas_Proposta_PropostaId",
                table: "Sintomas");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizador_Role_RoleID",
                table: "Utilizador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Utilizador",
                table: "Utilizador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposta",
                table: "Proposta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Higrometria",
                table: "Higrometria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Volumes",
                table: "Volumes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tipotratamentos",
                table: "Tipotratamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tipoobras",
                table: "Tipoobras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tipojanelas",
                table: "Tipojanelas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sintomas",
                table: "Sintomas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Qualidadears",
                table: "Qualidadears");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dadosgerals",
                table: "Dadosgerals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Construtivos",
                table: "Construtivos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Calculovolumes",
                table: "Calculovolumes");

            migrationBuilder.DropColumn(
                name: "Localidade",
                table: "Clientes");

            migrationBuilder.RenameTable(
                name: "Utilizador",
                newName: "utilizador");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "role");

            migrationBuilder.RenameTable(
                name: "Proposta",
                newName: "proposta");

            migrationBuilder.RenameTable(
                name: "Higrometria",
                newName: "higrometria");

            migrationBuilder.RenameTable(
                name: "Volumes",
                newName: "volume");

            migrationBuilder.RenameTable(
                name: "Tipotratamentos",
                newName: "tipotratamento");

            migrationBuilder.RenameTable(
                name: "Tipoobras",
                newName: "tipoobra");

            migrationBuilder.RenameTable(
                name: "Tipojanelas",
                newName: "tipojanela");

            migrationBuilder.RenameTable(
                name: "Sintomas",
                newName: "sintoma");

            migrationBuilder.RenameTable(
                name: "Qualidadears",
                newName: "qualidadear");

            migrationBuilder.RenameTable(
                name: "Dadosgerals",
                newName: "dadosgeral");

            migrationBuilder.RenameTable(
                name: "Construtivos",
                newName: "construtivo");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "cliente");

            migrationBuilder.RenameTable(
                name: "Calculovolumes",
                newName: "calculovolume");

            migrationBuilder.RenameIndex(
                name: "IX_Utilizador_RoleID",
                table: "utilizador",
                newName: "IX_utilizador_RoleID");

            migrationBuilder.RenameIndex(
                name: "IX_Proposta_UtilizadorId",
                table: "proposta",
                newName: "IX_proposta_UtilizadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposta_ClienteId",
                table: "proposta",
                newName: "IX_proposta_ClienteId");

            migrationBuilder.RenameColumn(
                name: "UtilizadorId",
                table: "PasswordResetToken",
                newName: "UtilizadorID");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetToken_UtilizadorId",
                table: "PasswordResetToken",
                newName: "IX_PasswordResetToken_UtilizadorID");

            migrationBuilder.RenameIndex(
                name: "IX_Higrometria_PropostaId",
                table: "higrometria",
                newName: "IX_higrometria_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_Higrometria_DadosGeralId",
                table: "higrometria",
                newName: "IX_higrometria_DadosGeralId");

            migrationBuilder.RenameIndex(
                name: "IX_Sintomas_PropostaId",
                table: "sintoma",
                newName: "IX_sintoma_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_Sintomas_DadosGeralId",
                table: "sintoma",
                newName: "IX_sintoma_DadosGeralId");

            migrationBuilder.RenameIndex(
                name: "IX_Qualidadears_VolumeId",
                table: "qualidadear",
                newName: "IX_qualidadear_VolumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Qualidadears_DadosGeralId",
                table: "qualidadear",
                newName: "IX_qualidadear_DadosGeralId");

            migrationBuilder.RenameIndex(
                name: "IX_Dadosgerals_TipoTratamentoId",
                table: "dadosgeral",
                newName: "IX_dadosgeral_TipoTratamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Dadosgerals_TipoObraId",
                table: "dadosgeral",
                newName: "IX_dadosgeral_TipoObraId");

            migrationBuilder.RenameIndex(
                name: "IX_Dadosgerals_PropostaId",
                table: "dadosgeral",
                newName: "IX_dadosgeral_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_Dadosgerals_DgTipoJanelaId",
                table: "dadosgeral",
                newName: "IX_dadosgeral_DgTipoJanelaId");

            migrationBuilder.RenameIndex(
                name: "IX_Construtivos_PropostaId",
                table: "construtivo",
                newName: "IX_construtivo_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_LocalidadeId",
                table: "cliente",
                newName: "IX_cliente_LocalidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Calculovolumes_VolumeId",
                table: "calculovolume",
                newName: "IX_calculovolume_VolumeId");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "utilizador",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Regiao",
                table: "Localidades",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NomeLocalidade",
                table: "Localidades",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone2",
                table: "cliente",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone1",
                table: "cliente",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Observacoes",
                table: "cliente",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroFiscal",
                table: "cliente",
                type: "varchar(9)",
                maxLength: 9,
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "cliente",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "cliente",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "cliente",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Dtnascimento",
                table: "cliente",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Cp4",
                table: "cliente",
                type: "varchar(4)",
                maxLength: 4,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Cp3",
                table: "cliente",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Apelido",
                table: "cliente",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_utilizador",
                table: "utilizador",
                column: "UtilizadorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role",
                table: "role",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposta",
                table: "proposta",
                column: "PropostaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_higrometria",
                table: "higrometria",
                column: "HigrometriaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_volume",
                table: "volume",
                column: "VolumeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tipotratamento",
                table: "tipotratamento",
                column: "TipoTratamentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tipoobra",
                table: "tipoobra",
                column: "TipoObraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tipojanela",
                table: "tipojanela",
                column: "TipoJanelaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sintoma",
                table: "sintoma",
                column: "SintomaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_qualidadear",
                table: "qualidadear",
                column: "QualidadeArId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dadosgeral",
                table: "dadosgeral",
                column: "DadosGeralId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_construtivo",
                table: "construtivo",
                column: "ConstrutivoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cliente",
                table: "cliente",
                column: "ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_calculovolume",
                table: "calculovolume",
                column: "CalculoVolumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_calculovolume_volume_VolumeId",
                table: "calculovolume",
                column: "VolumeId",
                principalTable: "volume",
                principalColumn: "VolumeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cliente_Localidades_LocalidadeId",
                table: "cliente",
                column: "LocalidadeId",
                principalTable: "Localidades",
                principalColumn: "LocalidadeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_construtivo_proposta_PropostaId",
                table: "construtivo",
                column: "PropostaId",
                principalTable: "proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dadosgeral_proposta_PropostaId",
                table: "dadosgeral",
                column: "PropostaId",
                principalTable: "proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dadosgeral_tipojanela_DgTipoJanelaId",
                table: "dadosgeral",
                column: "DgTipoJanelaId",
                principalTable: "tipojanela",
                principalColumn: "TipoJanelaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dadosgeral_tipoobra_TipoObraId",
                table: "dadosgeral",
                column: "TipoObraId",
                principalTable: "tipoobra",
                principalColumn: "TipoObraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dadosgeral_tipotratamento_TipoTratamentoId",
                table: "dadosgeral",
                column: "TipoTratamentoId",
                principalTable: "tipotratamento",
                principalColumn: "TipoTratamentoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_higrometria_dadosgeral_DadosGeralId",
                table: "higrometria",
                column: "DadosGeralId",
                principalTable: "dadosgeral",
                principalColumn: "DadosGeralId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_higrometria_proposta_PropostaId",
                table: "higrometria",
                column: "PropostaId",
                principalTable: "proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetToken_utilizador_UtilizadorID",
                table: "PasswordResetToken",
                column: "UtilizadorID",
                principalTable: "utilizador",
                principalColumn: "UtilizadorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposta_cliente_ClienteId",
                table: "proposta",
                column: "ClienteId",
                principalTable: "cliente",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposta_utilizador_UtilizadorId",
                table: "proposta",
                column: "UtilizadorId",
                principalTable: "utilizador",
                principalColumn: "UtilizadorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_qualidadear_dadosgeral_DadosGeralId",
                table: "qualidadear",
                column: "DadosGeralId",
                principalTable: "dadosgeral",
                principalColumn: "DadosGeralId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_qualidadear_volume_VolumeId",
                table: "qualidadear",
                column: "VolumeId",
                principalTable: "volume",
                principalColumn: "VolumeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sintoma_dadosgeral_DadosGeralId",
                table: "sintoma",
                column: "DadosGeralId",
                principalTable: "dadosgeral",
                principalColumn: "DadosGeralId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sintoma_proposta_PropostaId",
                table: "sintoma",
                column: "PropostaId",
                principalTable: "proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_utilizador_role_RoleID",
                table: "utilizador",
                column: "RoleID",
                principalTable: "role",
                principalColumn: "RoleID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_calculovolume_volume_VolumeId",
                table: "calculovolume");

            migrationBuilder.DropForeignKey(
                name: "FK_cliente_Localidades_LocalidadeId",
                table: "cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_construtivo_proposta_PropostaId",
                table: "construtivo");

            migrationBuilder.DropForeignKey(
                name: "FK_dadosgeral_proposta_PropostaId",
                table: "dadosgeral");

            migrationBuilder.DropForeignKey(
                name: "FK_dadosgeral_tipojanela_DgTipoJanelaId",
                table: "dadosgeral");

            migrationBuilder.DropForeignKey(
                name: "FK_dadosgeral_tipoobra_TipoObraId",
                table: "dadosgeral");

            migrationBuilder.DropForeignKey(
                name: "FK_dadosgeral_tipotratamento_TipoTratamentoId",
                table: "dadosgeral");

            migrationBuilder.DropForeignKey(
                name: "FK_higrometria_dadosgeral_DadosGeralId",
                table: "higrometria");

            migrationBuilder.DropForeignKey(
                name: "FK_higrometria_proposta_PropostaId",
                table: "higrometria");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetToken_utilizador_UtilizadorID",
                table: "PasswordResetToken");

            migrationBuilder.DropForeignKey(
                name: "FK_proposta_cliente_ClienteId",
                table: "proposta");

            migrationBuilder.DropForeignKey(
                name: "FK_proposta_utilizador_UtilizadorId",
                table: "proposta");

            migrationBuilder.DropForeignKey(
                name: "FK_qualidadear_dadosgeral_DadosGeralId",
                table: "qualidadear");

            migrationBuilder.DropForeignKey(
                name: "FK_qualidadear_volume_VolumeId",
                table: "qualidadear");

            migrationBuilder.DropForeignKey(
                name: "FK_sintoma_dadosgeral_DadosGeralId",
                table: "sintoma");

            migrationBuilder.DropForeignKey(
                name: "FK_sintoma_proposta_PropostaId",
                table: "sintoma");

            migrationBuilder.DropForeignKey(
                name: "FK_utilizador_role_RoleID",
                table: "utilizador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_utilizador",
                table: "utilizador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_role",
                table: "role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposta",
                table: "proposta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_higrometria",
                table: "higrometria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_volume",
                table: "volume");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tipotratamento",
                table: "tipotratamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tipoobra",
                table: "tipoobra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tipojanela",
                table: "tipojanela");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sintoma",
                table: "sintoma");

            migrationBuilder.DropPrimaryKey(
                name: "PK_qualidadear",
                table: "qualidadear");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dadosgeral",
                table: "dadosgeral");

            migrationBuilder.DropPrimaryKey(
                name: "PK_construtivo",
                table: "construtivo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cliente",
                table: "cliente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_calculovolume",
                table: "calculovolume");

            migrationBuilder.RenameTable(
                name: "utilizador",
                newName: "Utilizador");

            migrationBuilder.RenameTable(
                name: "role",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "proposta",
                newName: "Proposta");

            migrationBuilder.RenameTable(
                name: "higrometria",
                newName: "Higrometria");

            migrationBuilder.RenameTable(
                name: "volume",
                newName: "Volumes");

            migrationBuilder.RenameTable(
                name: "tipotratamento",
                newName: "Tipotratamentos");

            migrationBuilder.RenameTable(
                name: "tipoobra",
                newName: "Tipoobras");

            migrationBuilder.RenameTable(
                name: "tipojanela",
                newName: "Tipojanelas");

            migrationBuilder.RenameTable(
                name: "sintoma",
                newName: "Sintomas");

            migrationBuilder.RenameTable(
                name: "qualidadear",
                newName: "Qualidadears");

            migrationBuilder.RenameTable(
                name: "dadosgeral",
                newName: "Dadosgerals");

            migrationBuilder.RenameTable(
                name: "construtivo",
                newName: "Construtivos");

            migrationBuilder.RenameTable(
                name: "cliente",
                newName: "Clientes");

            migrationBuilder.RenameTable(
                name: "calculovolume",
                newName: "Calculovolumes");

            migrationBuilder.RenameIndex(
                name: "IX_utilizador_RoleID",
                table: "Utilizador",
                newName: "IX_Utilizador_RoleID");

            migrationBuilder.RenameIndex(
                name: "IX_proposta_UtilizadorId",
                table: "Proposta",
                newName: "IX_Proposta_UtilizadorId");

            migrationBuilder.RenameIndex(
                name: "IX_proposta_ClienteId",
                table: "Proposta",
                newName: "IX_Proposta_ClienteId");

            migrationBuilder.RenameColumn(
                name: "UtilizadorID",
                table: "PasswordResetToken",
                newName: "UtilizadorId");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetToken_UtilizadorID",
                table: "PasswordResetToken",
                newName: "IX_PasswordResetToken_UtilizadorId");

            migrationBuilder.RenameIndex(
                name: "IX_higrometria_PropostaId",
                table: "Higrometria",
                newName: "IX_Higrometria_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_higrometria_DadosGeralId",
                table: "Higrometria",
                newName: "IX_Higrometria_DadosGeralId");

            migrationBuilder.RenameIndex(
                name: "IX_sintoma_PropostaId",
                table: "Sintomas",
                newName: "IX_Sintomas_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_sintoma_DadosGeralId",
                table: "Sintomas",
                newName: "IX_Sintomas_DadosGeralId");

            migrationBuilder.RenameIndex(
                name: "IX_qualidadear_VolumeId",
                table: "Qualidadears",
                newName: "IX_Qualidadears_VolumeId");

            migrationBuilder.RenameIndex(
                name: "IX_qualidadear_DadosGeralId",
                table: "Qualidadears",
                newName: "IX_Qualidadears_DadosGeralId");

            migrationBuilder.RenameIndex(
                name: "IX_dadosgeral_TipoTratamentoId",
                table: "Dadosgerals",
                newName: "IX_Dadosgerals_TipoTratamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_dadosgeral_TipoObraId",
                table: "Dadosgerals",
                newName: "IX_Dadosgerals_TipoObraId");

            migrationBuilder.RenameIndex(
                name: "IX_dadosgeral_PropostaId",
                table: "Dadosgerals",
                newName: "IX_Dadosgerals_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_dadosgeral_DgTipoJanelaId",
                table: "Dadosgerals",
                newName: "IX_Dadosgerals_DgTipoJanelaId");

            migrationBuilder.RenameIndex(
                name: "IX_construtivo_PropostaId",
                table: "Construtivos",
                newName: "IX_Construtivos_PropostaId");

            migrationBuilder.RenameIndex(
                name: "IX_cliente_LocalidadeId",
                table: "Clientes",
                newName: "IX_Clientes_LocalidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_calculovolume_VolumeId",
                table: "Calculovolumes",
                newName: "IX_Calculovolumes_VolumeId");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Utilizador",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Regiao",
                table: "Localidades",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "NomeLocalidade",
                table: "Localidades",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<long>(
                name: "Telefone2",
                table: "Clientes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<long>(
                name: "Telefone1",
                table: "Clientes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Observacoes",
                keyValue: null,
                column: "Observacoes",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Observacoes",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "NumeroFiscal",
                keyValue: null,
                column: "NumeroFiscal",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroFiscal",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(9)",
                oldMaxLength: 9,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Email",
                keyValue: null,
                column: "Email",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Dtnascimento",
                table: "Clientes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cp4",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4)",
                oldMaxLength: 4)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Cp3",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Apelido",
                keyValue: null,
                column: "Apelido",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Apelido",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.AddColumn<string>(
                name: "Localidade",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Utilizador",
                table: "Utilizador",
                column: "UtilizadorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposta",
                table: "Proposta",
                column: "PropostaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Higrometria",
                table: "Higrometria",
                column: "HigrometriaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Volumes",
                table: "Volumes",
                column: "VolumeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tipotratamentos",
                table: "Tipotratamentos",
                column: "TipoTratamentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tipoobras",
                table: "Tipoobras",
                column: "TipoObraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tipojanelas",
                table: "Tipojanelas",
                column: "TipoJanelaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sintomas",
                table: "Sintomas",
                column: "SintomaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Qualidadears",
                table: "Qualidadears",
                column: "QualidadeArId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dadosgerals",
                table: "Dadosgerals",
                column: "DadosGeralId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Construtivos",
                table: "Construtivos",
                column: "ConstrutivoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Calculovolumes",
                table: "Calculovolumes",
                column: "CalculoVolumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calculovolumes_Volumes_VolumeId",
                table: "Calculovolumes",
                column: "VolumeId",
                principalTable: "Volumes",
                principalColumn: "VolumeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Localidades_LocalidadeId",
                table: "Clientes",
                column: "LocalidadeId",
                principalTable: "Localidades",
                principalColumn: "LocalidadeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Construtivos_Proposta_PropostaId",
                table: "Construtivos",
                column: "PropostaId",
                principalTable: "Proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dadosgerals_Proposta_PropostaId",
                table: "Dadosgerals",
                column: "PropostaId",
                principalTable: "Proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dadosgerals_Tipojanelas_DgTipoJanelaId",
                table: "Dadosgerals",
                column: "DgTipoJanelaId",
                principalTable: "Tipojanelas",
                principalColumn: "TipoJanelaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dadosgerals_Tipoobras_TipoObraId",
                table: "Dadosgerals",
                column: "TipoObraId",
                principalTable: "Tipoobras",
                principalColumn: "TipoObraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dadosgerals_Tipotratamentos_TipoTratamentoId",
                table: "Dadosgerals",
                column: "TipoTratamentoId",
                principalTable: "Tipotratamentos",
                principalColumn: "TipoTratamentoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Higrometria_Dadosgerals_DadosGeralId",
                table: "Higrometria",
                column: "DadosGeralId",
                principalTable: "Dadosgerals",
                principalColumn: "DadosGeralId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Higrometria_Proposta_PropostaId",
                table: "Higrometria",
                column: "PropostaId",
                principalTable: "Proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetToken_Utilizador_UtilizadorId",
                table: "PasswordResetToken",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "UtilizadorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposta_Clientes_ClienteId",
                table: "Proposta",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposta_Utilizador_UtilizadorId",
                table: "Proposta",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "UtilizadorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualidadears_Dadosgerals_DadosGeralId",
                table: "Qualidadears",
                column: "DadosGeralId",
                principalTable: "Dadosgerals",
                principalColumn: "DadosGeralId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualidadears_Volumes_VolumeId",
                table: "Qualidadears",
                column: "VolumeId",
                principalTable: "Volumes",
                principalColumn: "VolumeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sintomas_Dadosgerals_DadosGeralId",
                table: "Sintomas",
                column: "DadosGeralId",
                principalTable: "Dadosgerals",
                principalColumn: "DadosGeralId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sintomas_Proposta_PropostaId",
                table: "Sintomas",
                column: "PropostaId",
                principalTable: "Proposta",
                principalColumn: "PropostaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizador_Role_RoleID",
                table: "Utilizador",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "RoleID");
        }
    }
}
