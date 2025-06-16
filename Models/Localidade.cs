using System;
using System.Collections.Generic;

namespace MSPremiumProject.Models;

public partial class Localidade
{
    public ulong LocalidadeId { get; set; }

    public ulong PaisId { get; set; }

    public string NomeLocalidade { get; set; } = null!;

    public string Regiao { get; set; } = null!;

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual Pai Pais { get; set; } = null!;
}
