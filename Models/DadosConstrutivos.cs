using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("DadosConstrutivos")]
public partial class DadosConstrutivos
{
    public DadosConstrutivos()
    {
        Janelas = new HashSet<Janela>();
    }

    [Key]
    public ulong Id { get; set; }
    public DateTime? DataVisita { get; set; }
    public int? AnoConstrucao { get; set; }
    public decimal? AreaM2 { get; set; }
    public int? NumeroAndares { get; set; }
    public int? NumeroHabitantes { get; set; }
    public string? Localidade { get; set; }
    public int? Altitude { get; set; }
    public string? TipoFachada { get; set; }
    public string? OrientacaoFachada { get; set; }
    public string? CoberturaFachadaPrincipal { get; set; }
    public string? CoberturaFachadaPosterior { get; set; }
    public bool TratamentoHidrofugacao { get; set; }
    public string? IsolamentoCamara { get; set; }
    public string? IsolamentoInterno { get; set; }
    public string? TipoAquecimento { get; set; }

    public virtual ICollection<Janela> Janelas { get; set; }
    public virtual DadosGerais? DadosGerais { get; set; }
}