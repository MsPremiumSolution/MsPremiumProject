using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Sintomatologia")]
public partial class Sintomatologia
{
    [Key]
    public ulong Id { get; set; }
    public bool Fungos { get; set; }
    public bool Cheiros { get; set; }
    public bool MofoEmRoupasArmarios { get; set; }
    public bool CondensacaoNasJanelas { get; set; }
    public bool ConsumoExcessivoAquecimento { get; set; }
    public bool Alergias { get; set; }
    public bool ProblemasRespiratorios { get; set; }
    public bool GasRadao { get; set; }
    public bool EsporosEmSuperficies { get; set; }

    public virtual DadosGerais? DadosGerais { get; set; }
}