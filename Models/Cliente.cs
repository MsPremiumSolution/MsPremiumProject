using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Cliente
{
    [Key]
    public ulong ClienteId { get; set; }

    public string Nome { get; set; } = null!;

    public string Apelido { get; set; } = null!;

    public string Morada { get; set; } = null!;

    public string Cp4 { get; set; } = null!;

    public string Cp3 { get; set; } = null!;

    public ulong LocalidadeId { get; set; }

    public string Localidade { get; set; } = null!;

    public string NumeroFiscal { get; set; } = null!;

    public string Observacoes { get; set; } = null!;

    public string Email { get; set; } = null!;

    public long Telefone1 { get; set; }

    public long Telefone2 { get; set; }

    public DateOnly Dtnascimento { get; set; }

    public virtual Localidade LocalidadeNavigation { get; set; } = null!;

    public virtual ICollection<Propostum> Proposta { get; set; } = new List<Propostum>();
}
