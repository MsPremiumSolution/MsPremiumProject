using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Volume
{
    [Key]
    public ulong VolumeId { get; set; }

    public decimal VolumeTotal { get; set; }

    public decimal SuperficieTotal { get; set; }

    public virtual ICollection<Calculovolume> Calculovolumes { get; set; } = new List<Calculovolume>();

    public virtual ICollection<Qualidadear> Qualidadears { get; set; } = new List<Qualidadear>();
}
