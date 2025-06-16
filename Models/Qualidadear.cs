using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Qualidadear
{
    [Key]
    public ulong QualidadeArId { get; set; }

    public ulong ObjetivoId { get; set; }

    public ulong DadosGeralId { get; set; }

    public ulong VolumeId { get; set; }

    public virtual Dadosgeral DadosGeral { get; set; } = null!;

    public virtual Volume Volume { get; set; } = null!;
}
