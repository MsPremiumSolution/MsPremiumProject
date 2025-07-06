// File: MSPremiumProject/Models/OrcamentoArLinha.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("OrcamentoArLinhas")]
public class OrcamentoArLinha
{
    [Key]
    public ulong Id { get; set; }

    [Required]
    public ulong OrcamentoArId { get; set; } // Chave estrangeira para o orçamento pai

    [Required]
    [StringLength(100)]
    public string CodigoItem { get; set; } // Um código único para cada item (ex: "PROJ_CONTROLETECNICO")

    [Required]
    [StringLength(255)]
    public string Descricao { get; set; } // Descrição do item (ex: "Controle técnico")

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Quantidade { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecoUnitario { get; set; } // Preço no momento da criação

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalLinha { get; set; }

    public virtual OrcamentoAr OrcamentoAr { get; set; }
}