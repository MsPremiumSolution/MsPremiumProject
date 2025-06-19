// File: Models/Localidade.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Adicione este using
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models
{
    public partial class Localidade
    {
        [Key]
        public ulong LocalidadeId { get; set; }

        [Required(ErrorMessage = "É obrigatório selecionar um país.")]
        [Range(1, ulong.MaxValue, ErrorMessage = "Por favor, selecione um país válido.")] // Garante que não é 0
        public ulong PaisId { get; set; }

        [Required(ErrorMessage = "O nome da localidade é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome da localidade não pode exceder 200 caracteres.")]
        public string NomeLocalidade { get; set; } = null!;

        [StringLength(200, ErrorMessage = "A região não pode exceder 200 caracteres.")]
        public string Regiao { get; set; } = null!; // Tornar [Required] se a região for obrigatória

        public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

        [ForeignKey("PaisId")]
        public virtual Pai Pais { get; set; } = null!;
    }
}