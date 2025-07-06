// File: MSPremiumProject/Models/OrcamentoAr.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("OrcamentoAr")]
public partial class OrcamentoAr
{
    [Key]
    public ulong Id { get; set; }

    // Campos de dados
    [Column(TypeName = "decimal(18, 2)")]
    public decimal M3VolumeHabitavel { get; set; }
    public int NumeroCompartimentos { get; set; }
    public int NumeroPisos { get; set; }
    public string FiltroManutencao { get; set; } // "G4" ou "F7"

    // Checkboxes (para sabermos o que foi selecionado)
    public bool HasControleTecnico { get; set; }
    public bool HasExecucaoProjeto { get; set; }
    public bool HasInstalacaoMaoDeObra { get; set; }
    public bool HasAdaptacaoSistema { get; set; }
    public bool HasAcessoriosExtras { get; set; }
    public bool HasVigilancia24h { get; set; }

    // Valores finais de cada seção (PVP)
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorProjeto { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorFabricacao { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorConfiguracaoFabricacao { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorImplementacaoTrabalho { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorPersonalizacao { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorManutencao { get; set; }

    public virtual QualidadeDoAr? QualidadeDoAr { get; set; }
}