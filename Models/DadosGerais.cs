using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("DadosGerais")]
public partial class DadosGerais
{
    [Key]
    public ulong Id { get; set; }
    public ulong SintomalogiaId { get; set; }
    public ulong HigrometriaId { get; set; }
    public ulong DadosConstrutivosId { get; set; }

    [ForeignKey("SintomalogiaId")]
    public virtual Sintomatologia Sintomatologia { get; set; } = null!;
    [ForeignKey("HigrometriaId")]
    public virtual Higrometria Higrometria { get; set; } = null!;
    [ForeignKey("DadosConstrutivosId")]
    public virtual DadosConstrutivos DadosConstrutivo { get; set; } = null!;

    public virtual QualidadeDoAr? QualidadeDoAr { get; set; }
}