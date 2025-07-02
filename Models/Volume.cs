using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Volumes")]
public partial class Volume
{
    [Key]
    public ulong Id { get; set; }
    public ulong QualidadeDoArId { get; set; } // <<-- MUDANÇA IMPORTANTE
    public decimal Altura { get; set; }
    public decimal? Largura { get; set; }
    public decimal? Comprimento { get; set; }

    [ForeignKey("QualidadeDoArId")]
    public virtual QualidadeDoAr QualidadeDoAr { get; set; } = null!;
}