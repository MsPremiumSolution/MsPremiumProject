using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Propostum
{
    [Key]
    public ulong PropostaId { get; set; }

    public ulong ClienteId { get; set; }

    public DateTime DataProposta { get; set; }

    public DateTime DataAceitacao { get; set; }

    /// <summary>
    /// Codigo composto por AAAAMMMDD0000PropostaID
    /// </summary>
    public string CodigoProposta { get; set; } = null!;

    public ulong UtilizadorId { get; set; }

    public string Estado { get; set; } = null!;

    public decimal ValorObra { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<Construtivo> Construtivos { get; set; } = new List<Construtivo>();

    public virtual ICollection<Dadosgeral> Dadosgerals { get; set; } = new List<Dadosgeral>();

    public virtual ICollection<Higrometrium> Higrometria { get; set; } = new List<Higrometrium>();

    public virtual ICollection<Sintoma> Sintomas { get; set; } = new List<Sintoma>();

    public virtual Utilizador Utilizador { get; set; } = null!;
}
