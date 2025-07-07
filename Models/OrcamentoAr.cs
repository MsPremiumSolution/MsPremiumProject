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

    // Campos de dados
    [Column(TypeName = "decimal(18, 2)")]
    public decimal M3VolumeHabitavel { get; set; }
    public int NumeroCompartimentos { get; set; }
    public int NumeroPisos { get; set; }
    public string FiltroManutencao { get; set; }

    // Totais calculados
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorTotalDetalhe { get; set; } // Subtotal da página anterior

    // NOVAS PROPRIEDADES PARA O RESUMO
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TaxaIva { get; set; } // Ex: 6.00 para 6%

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorIva { get; set; }

    public bool DetalheConcluido { get; set; }

    public bool ResumoConcluido { get; set; } = false;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalFinalComIva { get; set; }


    public virtual QualidadeDoAr? QualidadeDoAr { get; set; }
    public virtual ICollection<OrcamentoArLinha> LinhasOrcamento { get; set; } = new List<OrcamentoArLinha>();
}