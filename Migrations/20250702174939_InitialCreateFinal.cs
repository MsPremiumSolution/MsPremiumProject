using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSPremiumProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DadosConstrutivos",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DataVisita = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AnoConstrucao = table.Column<int>(type: "int", nullable: true),
                    AreaM2 = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    NumeroAndares = table.Column<int>(type: "int", nullable: true),
                    NumeroHabitantes = table.Column<int>(type: "int", nullable: true),
                    Localidade = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Altitude = table.Column<int>(type: "int", nullable: true),
                    TipoFachada = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrientacaoFachada = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoberturaFachadaPrincipal = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoberturaFachadaPosterior = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TratamentoHidrofugacao = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsolamentoCamara = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsolamentoInterno = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoAquecimento = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosConstrutivos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Higrometria",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HumidadeRelativaExterior = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TemperaturaExterior = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    HumidadeRelativaInterior = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TemperaturaInterior = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TemperaturaParedesInternas = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TemperaturaPontoOrvalho = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    PontoDeOrvalho = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    pontos_frios = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    NivelCO2 = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    NivelTCOV = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    NivelHCHO = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    DataLoggerSensores = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Higrometria", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Objetivos",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsolamentoExternoSATE = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsolamentoInteriorPladur = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InjeccaoCamaraArPoliuretano = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TrituracaoCorticaTriturada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AplicacaoTintaTermica = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ImpermeabilizacaoFachadas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TubagemParedesInfiltracao = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InjeccaoParedesAccaoCapilar = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EvacuacaoHumidadeExcesso = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objetivos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "OrcamentoAr",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ValorProjeto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ValorFabricacao = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ConfiguracaoFabricacao = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ImplementacaoTrabalho = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Personalizacao = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Manutencao = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoAr", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    PaisId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NomePais = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CodigoIso = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.PaisId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Sintomatologia",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fungos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Cheiros = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MofoEmRoupasArmarios = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CondensacaoNasJanelas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConsumoExcessivoAquecimento = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Alergias = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ProblemasRespiratorios = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GasRadao = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsporosEmSuperficies = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sintomatologia", x => x.Id);
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
                name: "Janelas",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DadosConstrutivosId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TipoJanela = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Material = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoVidro = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroUnidades = table.Column<int>(type: "int", nullable: true),
                    PossuiJanelasDuplas = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    PossuiRPT = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    PossuiCaixaPersiana = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Janelas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Janelas_DadosConstrutivos_DadosConstrutivosId",
                        column: x => x.DadosConstrutivosId,
                        principalTable: "DadosConstrutivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Regiao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.LocalidadeId);
                    table.ForeignKey(
                        name: "FK_Localidades_Paises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Paises",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    UtilizadorID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Login = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PWP = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dtnascimento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.UtilizadorID);
                    table.ForeignKey(
                        name: "FK_Utilizadores_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "DadosGerais",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SintomalogiaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    HigrometriaId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DadosConstrutivosId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TipoObraId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    TipoTratamentoId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosGerais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DadosGerais_DadosConstrutivos_DadosConstrutivosId",
                        column: x => x.DadosConstrutivosId,
                        principalTable: "DadosConstrutivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DadosGerais_Higrometria_HigrometriaId",
                        column: x => x.HigrometriaId,
                        principalTable: "Higrometria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DadosGerais_Sintomatologia_SintomalogiaId",
                        column: x => x.SintomalogiaId,
                        principalTable: "Sintomatologia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DadosGerais_Tipoobras_TipoObraId",
                        column: x => x.TipoObraId,
                        principalTable: "Tipoobras",
                        principalColumn: "TipoObraId");
                    table.ForeignKey(
                        name: "FK_DadosGerais_Tipotratamentos_TipoTratamentoId",
                        column: x => x.TipoTratamentoId,
                        principalTable: "Tipotratamentos",
                        principalColumn: "TipoTratamentoId");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apelido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Morada = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cp4 = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cp3 = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CodigoPostalEstrangeiro = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LocalidadeId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    NumeroFiscal = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observacoes = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefone1 = table.Column<long>(type: "bigint", nullable: true),
                    Telefone2 = table.Column<long>(type: "bigint", nullable: true),
                    Dtnascimento = table.Column<DateOnly>(type: "date", nullable: true)
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
                name: "PasswordResetTokens",
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
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "UtilizadorID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "QualidadeDoAr",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DadosGeraisId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ObjetivosId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    OrcamentoArId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualidadeDoAr", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualidadeDoAr_DadosGerais_DadosGeraisId",
                        column: x => x.DadosGeraisId,
                        principalTable: "DadosGerais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualidadeDoAr_Objetivos_ObjetivosId",
                        column: x => x.ObjetivosId,
                        principalTable: "Objetivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualidadeDoAr_OrcamentoAr_OrcamentoArId",
                        column: x => x.OrcamentoArId,
                        principalTable: "OrcamentoAr",
                        principalColumn: "Id",
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
                    ValorObra = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    QualidadeDoArId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    TratamentoEstruturalId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
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
                        name: "FK_Proposta_QualidadeDoAr_QualidadeDoArId",
                        column: x => x.QualidadeDoArId,
                        principalTable: "QualidadeDoAr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Proposta_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "UtilizadorID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "Volumes",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QualidadeDoArId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Altura = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Largura = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Comprimento = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Volumes_QualidadeDoAr_QualidadeDoArId",
                        column: x => x.QualidadeDoArId,
                        principalTable: "QualidadeDoAr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_LocalidadeId",
                table: "Clientes",
                column: "LocalidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_DadosGerais_DadosConstrutivosId",
                table: "DadosGerais",
                column: "DadosConstrutivosId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DadosGerais_HigrometriaId",
                table: "DadosGerais",
                column: "HigrometriaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DadosGerais_SintomalogiaId",
                table: "DadosGerais",
                column: "SintomalogiaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DadosGerais_TipoObraId",
                table: "DadosGerais",
                column: "TipoObraId");

            migrationBuilder.CreateIndex(
                name: "IX_DadosGerais_TipoTratamentoId",
                table: "DadosGerais",
                column: "TipoTratamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Janelas_DadosConstrutivosId",
                table: "Janelas",
                column: "DadosConstrutivosId");

            migrationBuilder.CreateIndex(
                name: "IX_Localidades_PaisId",
                table: "Localidades",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UtilizadorId",
                table: "PasswordResetTokens",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_ClienteId",
                table: "Proposta",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_QualidadeDoArId",
                table: "Proposta",
                column: "QualidadeDoArId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposta_UtilizadorId",
                table: "Proposta",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_QualidadeDoAr_DadosGeraisId",
                table: "QualidadeDoAr",
                column: "DadosGeraisId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualidadeDoAr_ObjetivosId",
                table: "QualidadeDoAr",
                column: "ObjetivosId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualidadeDoAr_OrcamentoArId",
                table: "QualidadeDoAr",
                column: "OrcamentoArId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_RoleID",
                table: "Utilizadores",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Volumes_QualidadeDoArId",
                table: "Volumes",
                column: "QualidadeDoArId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Janelas");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "Proposta");

            migrationBuilder.DropTable(
                name: "Volumes");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropTable(
                name: "QualidadeDoAr");

            migrationBuilder.DropTable(
                name: "Localidades");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "DadosGerais");

            migrationBuilder.DropTable(
                name: "Objetivos");

            migrationBuilder.DropTable(
                name: "OrcamentoAr");

            migrationBuilder.DropTable(
                name: "Paises");

            migrationBuilder.DropTable(
                name: "DadosConstrutivos");

            migrationBuilder.DropTable(
                name: "Higrometria");

            migrationBuilder.DropTable(
                name: "Sintomatologia");

            migrationBuilder.DropTable(
                name: "Tipoobras");

            migrationBuilder.DropTable(
                name: "Tipotratamentos");
        }
    }
}
