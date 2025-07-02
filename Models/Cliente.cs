// Ficheiro: Models/Cliente.cs (Versão Simplificada e Corrigida)

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models
{
    [Table("Clientes")]
    public partial class Cliente
    {
        [Key]
        public ulong ClienteId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = null!;

        [StringLength(100, ErrorMessage = "O apelido não pode exceder 100 caracteres.")]
        [Display(Name = "Apelido")]
        public string? Apelido { get; set; }

        [StringLength(150, ErrorMessage = "O Nome da Empresa deve ter no máximo 150 caracteres.")]
        [Display(Name = "Empresa (Opcional)")]
        public string? Empresa { get; set; }

        [Required(ErrorMessage = "A morada é obrigatória.")]
        [StringLength(255, ErrorMessage = "A morada não pode exceder 255 caracteres.")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = null!;

        [RegularExpression(@"^\d{4}$", ErrorMessage = "O CP4 deve ter 4 dígitos.")]
        [StringLength(4)]
        [Display(Name = "CP (4 Dígitos)")]
        public string? Cp4 { get; set; }

        [RegularExpression(@"^\d{3}$", ErrorMessage = "O CP3 deve ter 3 dígitos.")]
        [StringLength(3)]
        [Display(Name = "CP (3 Dígitos)")]
        public string? Cp3 { get; set; }

        [StringLength(20, ErrorMessage = "O código postal não pode exceder 20 caracteres.")]
        [Display(Name = "Código Postal Estrangeiro")]
        public string? CodigoPostalEstrangeiro { get; set; }

        [Required(ErrorMessage = "A localidade é obrigatória.")]
        [Display(Name = "Localidade")]
        public ulong LocalidadeId { get; set; }

        [Display(Name = "Número Fiscal (NIF)")]
        [StringLength(50, ErrorMessage = "O NIF não pode exceder 50 caracteres.")]
        public string? NumeroFiscal { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Telefone 1")]
        public long? Telefone1 { get; set; }

        [Display(Name = "Telefone 2")]
        public long? Telefone2 { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateOnly? Dtnascimento { get; set; }

        // --- Propriedades de Navegação ---
        [ForeignKey("LocalidadeId")]
        public virtual Localidade? LocalidadeNavigation { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; } = new List<Proposta>();
    }
}