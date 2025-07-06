// File: MSPremiumProject/Models/OrcamentoAr.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("OrcamentoAr")]
public partial class OrcamentoAr
{
    [Key]
    public ulong Id { get; set; }

    // Campos de dados que influenciam os cálculos, mas não são linhas de orçamento
    [Column(TypeName = "decimal(18, 2)")]
    public decimal M3VolumeHabitavel { get; set; }
    public int NumeroCompartimentos { get; set; }
    public int NumeroPisos { get; set; }
    public string FiltroManutencao { get; set; }

    public virtual QualidadeDoAr? QualidadeDoAr { get; set; }

    // Coleção de todas as linhas de itens deste orçamento
    public virtual ICollection<OrcamentoArLinha> LinhasOrcamento { get; set; } = new List<OrcamentoArLinha>();
}