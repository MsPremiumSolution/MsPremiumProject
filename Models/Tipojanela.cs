using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Tipojanela
{
    [Key]
    public ulong TipoJanelaId { get; set; }

    public string TipoJanela1 { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public virtual ICollection<Dadosgeral> Dadosgerals { get; set; } = new List<Dadosgeral>();
}
