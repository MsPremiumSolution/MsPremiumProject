using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("OrcamentoAr")]
public partial class OrcamentoAr
{
    [Key]
    public ulong Id { get; set; }
    public decimal ValorProjeto { get; set; }
    public decimal ValorFabricacao { get; set; }
    public decimal ConfiguracaoFabricacao { get; set; }
    public decimal ImplementacaoTrabalho { get; set; }
    public decimal Personalizacao { get; set; }
    public decimal Manutencao { get; set; }

    public virtual QualidadeDoAr? QualidadeDoAr { get; set; }
}