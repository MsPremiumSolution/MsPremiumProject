using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class CreateFullSchemaWithToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pais",
                columns: table => new
                {
                    PaisID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NomePais = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pais", x => x.PaisID);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            // ... (todas as outras CreateTable para Tipojanelas, Tipoobras, etc., permanecem iguais) ...
            migrationBuilder.CreateTable(
                name: "Tipojanelas",
                columns: table => new
                {
                    TipoJanelaId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TipoJanela1 = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipojanelas", x => x.TipoJanelaId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Tipoobras",
                columns: table => new
                {
                    TipoObraId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observacoes = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipoobras", x => x.TipoObraId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Tipotratamentos",
                columns: table => new
                {
                    TipoTratamentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TipoTratamentoNome = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipotratamentos", x => x.TipoTratamentoId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Volumes",
                columns: table => new
                {
                    VolumeId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VolumeTotal = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    SuperficieTotal = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volumes", x => x.VolumeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Localidades",
                columns: table => new
                {
                    LocalidadeId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PaisId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    NomeLocalidade = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Regiao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.LocalidadeId);
                    table.ForeignKey(
                        name: "FK_Localidades_pais_PaisId",
                        column: x => x.PaisId,
                        principalTable: "pais",
                        principalColumn: "PaisID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    UtilizadorID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleID = table.Column<ulong>(type: "bigint unsigned", nullable: false), // Assumindo que RoleID não é anulável
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PWP = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dtnascimento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.UtilizadorID);
                    table.ForeignKey(
                        name: "FK_Utilizador_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict); // ***** ALTERADO AQUI *****
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            // ... (todas as outras CreateTable para Calculovolumes, Clientes, etc., permanecem iguais) ...
            // ... (incluindo PasswordResetToken) ...

            migrationBuilder.CreateTable(
                name: "Calculovolumes",
                columns: table => new
                {
                    CalculoVolumeId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VolumeId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    AlturaMetros = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    EstadiaDireta = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Estadia1 = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Largura = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Comprimento = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculovolumes", x => x.CalculoVolumeId);
                    table.ForeignKey(
                        name: "FK_Calculovolumes_Volumes_VolumeId",
                        column: x => x.VolumeId,
                        principalTable: "Volumes",
                        principalColumn: "VolumeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apelido = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Morada = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cp4 = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cp3 = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LocalidadeId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Localidade = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroFiscal = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observacoes = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefone1 = table.Column<long>(type: "bigint", nullable: false),
                    Telefone2 = table.Column<long>(type: "bigint", nullable: false),
                    Dtnascimento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                    table.ForeignKey(
                        name: "FK_Clientes_Localidades_LocalidadeId",
                        column: x => x.LocalidadeId,
                        principalTable: "Localidades",
                        principalColumn: "LocalidadeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "PasswordResetToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UtilizadorId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TokenValue = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsUsed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetToken_Utilizador_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizador",
                        principalColumn: "UtilizadorID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Proposta",
                columns: table => new
                {
                    PropostaId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DataProposta = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataAceitacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CodigoProposta = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UtilizadorId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Estado = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorObra = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposta", x => x.PropostaId);
                    table.ForeignKey(
                        name: "FK_Proposta_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposta_Utilizador_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizador",
                        principalColumn: "UtilizadorID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            // ... (todas as outras CreateTable para Construtivos, Dadosgerals, etc., permanecem iguais) ...
            migrationBuilder.CreateTable(
                name: "Construtivos",
                columns: table => new
                {
                    ConstrutivoId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropostaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AnoConstrucao = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Area = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Nandares = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Nhabitantes = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Localidade = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Altitude = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechamentoTipoFachada = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechamentoOrientacao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechamentoCobertura = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechamentoCoberturaPosterior = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechamentoTratHidrofugacao = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Construtivos", x => x.ConstrutivoId);
                    table.ForeignKey(
                        name: "FK_Construtivos_Proposta_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Proposta",
                        principalColumn: "PropostaId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Dadosgerals",
                columns: table => new
                {
                    DadosGeralId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropostaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TipoObraId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TipoTratamentoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DgTipoFachada = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgOrientacao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgCoberturaFprincipal = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgCoberturaFposterior = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgTratamentoHidrofugacao = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DgIsolamentoCamera = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgIsolamentoInterno = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgIsolamentoAquecimento = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgTipoJanelaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DgTipoJanelaMaterial = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgTipoJanelaDuplas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DgTipoJanelaVidro = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DgTipoJanelaRpt = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DgTipoJanelaCaixasPersiana = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DgTipoJanelaUnidades = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dadosgerals", x => x.DadosGeralId);
                    table.ForeignKey(
                        name: "FK_Dadosgerals_Proposta_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Proposta",
                        principalColumn: "PropostaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dadosgerals_Tipojanelas_DgTipoJanelaId",
                        column: x => x.DgTipoJanelaId,
                        principalTable: "Tipojanelas",
                        principalColumn: "TipoJanelaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dadosgerals_Tipoobras_TipoObraId",
                        column: x => x.TipoObraId,
                        principalTable: "Tipoobras",
                        principalColumn: "TipoObraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dadosgerals_Tipotratamentos_TipoTratamentoId",
                        column: x => x.TipoTratamentoId,
                        principalTable: "Tipotratamentos",
                        principalColumn: "TipoTratamentoId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Higrometria",
                columns: table => new
                {
                    HigrometriaId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DadosGeralId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    PropostaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    HumidadeExterior = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TempExterior = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    HumidadeInterior = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TempInterior = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TempParedesInt = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TempPontosFrios = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PontoOrvalho = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NivelCo2 = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NivelTcov = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NivelHcho = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DataLogger = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Higrometria", x => x.HigrometriaId);
                    table.ForeignKey(
                        name: "FK_Higrometria_Dadosgerals_DadosGeralId",
                        column: x => x.DadosGeralId,
                        principalTable: "Dadosgerals",
                        principalColumn: "DadosGeralId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Higrometria_Proposta_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Proposta",
                        principalColumn: "PropostaId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Qualidadears",
                columns: table => new
                {
                    QualidadeArId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ObjetivoId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DadosGeralId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    VolumeId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualidadears", x => x.QualidadeArId);
                    table.ForeignKey(
                        name: "FK_Qualidadears_Dadosgerals_DadosGeralId",
                        column: x => x.DadosGeralId,
                        principalTable: "Dadosgerals",
                        principalColumn: "DadosGeralId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qualidadears_Volumes_VolumeId",
                        column: x => x.VolumeId,
                        principalTable: "Volumes",
                        principalColumn: "VolumeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Sintomas",
                columns: table => new
                {
                    SintomaId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DadosGeralId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    PropostaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Fungos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Cheiros = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MofosRoupasArmario = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CondensacaoJanelas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExcessoAquecimento = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Alergias = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GasRadon = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Esporos = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sintomas", x => x.SintomaId);
                    table.ForeignKey(
                        name: "FK_Sintomas_Dadosgerals_DadosGeralId",
                        column: x => x.DadosGeralId,
                        principalTable: "Dadosgerals",
                        principalColumn: "DadosGeralId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sintomas_Proposta_PropostaId",
                        column: x => x.PropostaId,
                        principalTable: "Proposta",
                        principalColumn: "PropostaId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");


            // --- ÍNDICES ---
            migrationBuilder.CreateIndex(
                name: "IX_Calculovolumes_VolumeId",
                table: "Calculovolumes",
                column: "VolumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_LocalidadeId",
                table: "Clientes",
                column: "LocalidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Construtivos_PropostaId",
                table: "Construtivos",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dadosgerals_DgTipoJanelaId",
                table: "Dadosgerals",
                column: "DgTipoJanelaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dadosgerals_PropostaId",
                table: "Dadosgerals",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dadosgerals_TipoObraId",
                table: "Dadosgerals",
                column: "TipoObraId");

            migrationBuilder.CreateIndex(
                name: "IX_Dadosgerals_TipoTratamentoId",
                table: "Dadosgerals",
                column: "TipoTratamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Higrometria_DadosGeralId",
                table: "Higrometria",
                column: "DadosGeralId");

            migrationBuilder.CreateIndex(
                name: "IX_Higrometria_PropostaId",
                table: "Higrometria",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_Localidades_PaisId",
                table: "Localidades",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetToken_TokenValue",
                table: "PasswordResetToken",
                column: "TokenValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetToken_UtilizadorId",
                table: "PasswordResetToken",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_ClienteId",
                table: "Proposta",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_UtilizadorId",
                table: "Proposta",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualidadears_DadosGeralId",
                table: "Qualidadears",
                column: "DadosGeralId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualidadears_VolumeId",
                table: "Qualidadears",
                column: "VolumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sintomas_DadosGeralId",
                table: "Sintomas",
                column: "DadosGeralId");

            migrationBuilder.CreateIndex(
                name: "IX_Sintomas_PropostaId",
                table: "Sintomas",
                column: "PropostaId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizador_RoleID",
                table: "Utilizador",
                column: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ... (o método Down permanece o mesmo que você forneceu, com todos os DropTable) ...
            migrationBuilder.DropTable(
                name: "Calculovolumes");

            migrationBuilder.DropTable(
                name: "Construtivos");

            migrationBuilder.DropTable(
                name: "Higrometria");

            migrationBuilder.DropTable(
                name: "PasswordResetToken");

            migrationBuilder.DropTable(
                name: "Qualidadears");

            migrationBuilder.DropTable(
                name: "Sintomas");

            migrationBuilder.DropTable(
                name: "Volumes");

            migrationBuilder.DropTable(
                name: "Dadosgerals");

            migrationBuilder.DropTable(
                name: "Proposta");

            migrationBuilder.DropTable(
                name: "Tipojanelas");

            migrationBuilder.DropTable(
                name: "Tipoobras");

            migrationBuilder.DropTable(
                name: "Tipotratamentos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Utilizador");

            migrationBuilder.DropTable(
                name: "Localidades");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "pais");
        }
    }
}