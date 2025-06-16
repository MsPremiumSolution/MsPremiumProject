using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Higrometrium
{
    [Key]
    public ulong HigrometriaId { get; set; }

    public ulong DadosGeralId { get; set; }

    public ulong PropostaId { get; set; }

    public decimal HumidadeExterior { get; set; }

    public decimal TempExterior { get; set; }

    public decimal HumidadeInterior { get; set; }

    public decimal TempInterior { get; set; }

    public decimal TempParedesInt { get; set; }

    public decimal TempPontosFrios { get; set; }

    public decimal PontoOrvalho { get; set; }

    public decimal NivelCo2 { get; set; }

    public decimal NivelTcov { get; set; }

    public decimal NivelHcho { get; set; }

    public string DataLogger { get; set; } = null!;

    public virtual Dadosgeral DadosGeral { get; set; } = null!;

    public virtual Propostum Proposta { get; set; } = null!;
}
