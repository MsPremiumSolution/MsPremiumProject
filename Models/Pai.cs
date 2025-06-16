using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Pai
{
    [Key]
    public ulong PaisId { get; set; }

    public string NomePais { get; set; } = null!;

    public virtual ICollection<Localidade> Localidades { get; set; } = new List<Localidade>();
}
