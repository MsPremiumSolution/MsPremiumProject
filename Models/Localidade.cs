using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Localidade
{
    [Key]
    public ulong LocalidadeId { get; set; }

    [Required(ErrorMessage = "É obrigatório selecionar um país.")]
    [Range(1, ulong.MaxValue, ErrorMessage = "País inválido.")] // Garante que não é 0
    public ulong PaisId { get; set; }

    [Required(ErrorMessage = "O nome da localidade é obrigatório.")]
    [StringLength(255)] // Defina um tamanho máximo
    public string NomeLocalidade { get; set; } = null!;

    [Required(ErrorMessage = "A região é obrigatória.")] // Se for obrigatória
    [StringLength(255)] // Defina um tamanho máximo
    public string Regiao { get; set; } = null!;

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual Pai Pais { get; set; } = null!;
}
