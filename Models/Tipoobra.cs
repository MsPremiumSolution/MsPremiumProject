using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Tipoobra
{
    [Key]
    public ulong TipoObraId { get; set; }

    public string Descricao { get; set; } = null!;

    public string Observacoes { get; set; } = null!;

    public virtual ICollection<DadosGerais> Dadosgerals { get; set; } = new List<DadosGerais>();
}
