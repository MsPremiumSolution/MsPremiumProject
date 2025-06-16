using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Tipotratamento
{
    [Key]
    public ulong TipoTratamentoId { get; set; }

    public string TipoTratamentoNome { get; set; } = null!;

    public virtual ICollection<Dadosgeral> Dadosgerals { get; set; } = new List<Dadosgeral>();
}
