using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Construtivo
{
    [Key]
    public ulong ConstrutivoId { get; set; }

    public ulong PropostaId { get; set; }

    public DateTime Data { get; set; }

    public decimal AnoConstrucao { get; set; }

    public decimal Area { get; set; }

    public decimal Nandares { get; set; }

    public decimal Nhabitantes { get; set; }

    public string Localidade { get; set; } = null!;

    public bool Altitude { get; set; }

    public string FechamentoTipoFachada { get; set; } = null!;

    public string FechamentoOrientacao { get; set; } = null!;

    public string FechamentoCobertura { get; set; } = null!;

    public string FechamentoCoberturaPosterior { get; set; } = null!;

    public bool FechamentoTratHidrofugacao { get; set; }

    public virtual Propostum Proposta { get; set; } = null!;
}
