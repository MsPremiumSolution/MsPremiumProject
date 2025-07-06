using MSPremiumProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("QualidadeDoAr")]
public partial class QualidadeDoAr
{
    public QualidadeDoAr()
    {
        Volumes = new HashSet<Volume>();
    }

    [Key]
    public ulong Id { get; set; }
    public ulong? DadosGeraisId { get; set; }
    public ulong? ObjetivosId { get; set; }
    public ulong? OrcamentoArId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal VolumeTotal { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SuperficieTotal { get; set; }

    [ForeignKey("DadosGeraisId")]
    public virtual DadosGerais? DadosGerais { get; set; } = null!;
    [ForeignKey("ObjetivosId")]
    public virtual Objetivos? Objetivos { get; set; } = null!;
    [ForeignKey("OrcamentoArId")]
    public virtual OrcamentoAr? OrcamentoAr { get; set; } = null!;

    public virtual ICollection<Volume> Volumes { get; set; }
    public virtual Proposta? Proposta { get; set; }
}