using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// using MSPremiumProject.Utils; // Não necessário aqui se Validate for movido
// using MSPremiumProject.Data; // Não necessário aqui se Validate for movido
// using Microsoft.EntityFrameworkCore; // Não necessário aqui se Validate for movido

// using System.Linq; // Não necessário aqui se Validate for movido

namespace MSPremiumProject.Models
{
    // Removido: : IValidatableObject
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

        [StringLength(255, ErrorMessage = "O nome da empresa não pode exceder 255 caracteres.")]
        [Display(Name = "Empresa")]
        public string? Empresa { get; set; }

        [Required(ErrorMessage = "A morada é obrigatória.")]
        [StringLength(255, ErrorMessage = "A morada não pode exceder 255 caracteres.")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = null!;

        // CAMPO NOVO: Para guardar o texto da localidade diretamente no Cliente
        
        [StringLength(100, ErrorMessage = "A localidade não pode exceder 100 caracteres.")]
        [Display(Name = "Localidade")] // Renomeado para coincidir com o formulário
        public string? NomeLocalidadeTexto { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "O CP4 deve ter 4 dígitos.")]
        [StringLength(4)]
        [Display(Name = "CP (4 Dígitos)")]
        public string? Cp4 { get; set; }

        [RegularExpression(@"^\d{3}$", ErrorMessage = "O CP3 deve ter 3 dígitos.")]
        [StringLength(3)]
        [Display(Name = "CP (3 Dígitos)")]
        public string? Cp3 { get; set; }

        [StringLength(20, ErrorMessage = "O código postal não pode exceder 20 caracteres.")]
        [Display(Name = "Código Postal")]
        public string? CodigoPostalEstrangeiro { get; set; }

        // A LocalidadeId continua a ser a chave estrangeira para a tabela Localidades,
        // usada para obter o país para validação de NIF/CP e para a relação com Localidade.
        [Required(ErrorMessage = "É necessário associar uma localidade válida.")]
        [Display(Name = "Localidade Associada (ID)")]
        public ulong LocalidadeId { get; set; }

        [Display(Name = "Número Fiscal (NIF)")]
        [StringLength(50, ErrorMessage = "O NIF não pode exceder 50 caracteres.")]
        public string? NumeroFiscal { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
        [StringLength(255, ErrorMessage = "O email não pode exceder 255 caracteres.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Telefone 1")]
        public long? Telefone1 { get; set; }

        [Display(Name = "Telefone 2")]
        public long? Telefone2 { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateOnly? Dtnascimento { get; set; }

        [ForeignKey("LocalidadeId")]
        public virtual Localidade? LocalidadeNavigation { get; set; }

        public virtual ICollection<Proposta> Proposta { get; set; } = new List<Proposta>();

        // Removido o método Validate
    }
}