using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Objetivos")]
public partial class Objetivos
{
    [Key]
    public ulong Id { get; set; }
    public bool IsolamentoExternoSATE { get; set; }
    public bool IsolamentoInteriorPladur { get; set; }
    public bool InjeccaoCamaraArPoliuretano { get; set; }
    public bool TrituracaoCorticaTriturada { get; set; }
    public bool AplicacaoTintaTermica { get; set; }
    public bool ImpermeabilizacaoFachadas { get; set; }
    public bool TubagemParedesInfiltracao { get; set; }
    public bool InjeccaoParedesAccaoCapilar { get; set; }
    public bool EvacuacaoHumidadeExcesso { get; set; }

    public virtual QualidadeDoAr? QualidadeDoAr { get; set; }
}