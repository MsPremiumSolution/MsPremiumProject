using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Calculovolume
{
    [Key]
    public ulong CalculoVolumeId { get; set; }

    public ulong VolumeId { get; set; }

    public decimal AlturaMetros { get; set; }

    public decimal EstadiaDireta { get; set; }

    public decimal Estadia1 { get; set; }

    public decimal Largura { get; set; }

    public decimal Comprimento { get; set; }

    public virtual Volume Volume { get; set; } = null!;
}
