using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Janelas")]
public partial class Janela
{
    [Key]
    public ulong Id { get; set; }
    public ulong DadosConstrutivosId { get; set; }
    public string? TipoJanela { get; set; }
    public string? Material { get; set; }
    public string? TipoVidro { get; set; }
    public int? NumeroUnidades { get; set; }
    public bool? PossuiJanelasDuplas { get; set; }
    public bool? PossuiRPT { get; set; }
    public bool? PossuiCaixaPersiana { get; set; }

    [ForeignKey("DadosConstrutivosId")]
    public virtual DadosConstrutivos DadosConstrutivo { get; set; } = null!;
}