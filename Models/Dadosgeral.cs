using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Dadosgeral
{
    [Key]
    public ulong DadosGeralId { get; set; }

    public ulong PropostaId { get; set; }

    public ulong TipoObraId { get; set; }

    public ulong TipoTratamentoId { get; set; }

    public string DgTipoFachada { get; set; } = null!;

    public string DgOrientacao { get; set; } = null!;

    public string DgCoberturaFprincipal { get; set; } = null!;

    public string DgCoberturaFposterior { get; set; } = null!;

    public bool DgTratamentoHidrofugacao { get; set; }

    public string DgIsolamentoCamera { get; set; } = null!;

    public string DgIsolamentoInterno { get; set; } = null!;

    public string DgIsolamentoAquecimento { get; set; } = null!;

    public ulong DgTipoJanelaId { get; set; }

    public string DgTipoJanelaMaterial { get; set; } = null!;

    public bool DgTipoJanelaDuplas { get; set; }

    public string DgTipoJanelaVidro { get; set; } = null!;

    public bool DgTipoJanelaRpt { get; set; }

    public bool DgTipoJanelaCaixasPersiana { get; set; }

    public decimal DgTipoJanelaUnidades { get; set; }

    public virtual Tipojanela DgTipoJanela { get; set; } = null!;

    public virtual ICollection<Higrometrium> Higrometria { get; set; } = new List<Higrometrium>();

    public virtual Propostum Proposta { get; set; } = null!;

    public virtual ICollection<Qualidadear> Qualidadears { get; set; } = new List<Qualidadear>();

    public virtual ICollection<Sintoma> Sintomas { get; set; } = new List<Sintoma>();

    public virtual Tipoobra TipoObra { get; set; } = null!;

    public virtual Tipotratamento TipoTratamento { get; set; } = null!;
}
