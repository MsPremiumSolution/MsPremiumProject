using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Sintoma
{
    [Key]
    public ulong SintomaId { get; set; }

    public ulong DadosGeralId { get; set; }

    public ulong PropostaId { get; set; }

    public bool Fungos { get; set; }

    public bool Cheiros { get; set; }

    public bool MofosRoupasArmario { get; set; }

    public bool CondensacaoJanelas { get; set; }

    public bool ExcessoAquecimento { get; set; }

    public bool Alergias { get; set; }

    public bool GasRadon { get; set; }

    public bool Esporos { get; set; }

    public virtual Dadosgeral DadosGeral { get; set; } = null!;

    public virtual Propostum Proposta { get; set; } = null!;
}
