// File: Models/Cliente.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSPremiumProject.Utils;
using MSPremiumProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MSPremiumProject.Models
{
    public partial class Cliente : IValidatableObject
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

        [Required(ErrorMessage = "A morada é obrigatória.")]
        [StringLength(255, ErrorMessage = "A morada não pode exceder 255 caracteres.")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = null!;

        // ***** ALTERAÇÃO 1: REMOVIDO [Required] *****
        [RegularExpression(@"^\d{4}$", ErrorMessage = "O CP4 deve ter 4 dígitos.")]
        [StringLength(4)]
        [Display(Name = "CP (4 Dígitos)")]
        public string? Cp4 { get; set; } // Alterado para string? para permitir nulo

        // ***** ALTERAÇÃO 1: REMOVIDO [Required] *****
        [RegularExpression(@"^\d{3}$", ErrorMessage = "O CP3 deve ter 3 dígitos.")]
        [StringLength(3)]
        [Display(Name = "CP (3 Dígitos)")]
        public string? Cp3 { get; set; } // Alterado para string? para permitir nulo

        // ***** ALTERAÇÃO 2: ADICIONADO NOVO CAMPO *****
        [StringLength(20, ErrorMessage = "O código postal não pode exceder 20 caracteres.")]
        [Display(Name = "Código Postal")]
        public string? CodigoPostalEstrangeiro { get; set; }

        [Required(ErrorMessage = "A localidade é obrigatória.")]
        [Display(Name = "ID da Localidade")]
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

        public virtual ICollection<Propostum> Proposta { get; set; } = new List<Propostum>();

        // ***** ALTERAÇÃO 3: MÉTODO VALIDATE ATUALIZADO *****
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            if (dbContext == null)
            {
                // Não é possível validar sem o contexto, retorna sem erros para não bloquear
                yield break;
            }

            string? paisCodigoIso = null;
            if (LocalidadeId > 0)
            {
                var localidadeDoCliente = dbContext.Localidades
                                              .Include(l => l.Pais)
                                              .AsNoTracking()
                                              .FirstOrDefault(l => l.LocalidadeId == this.LocalidadeId);
                if (localidadeDoCliente?.Pais != null)
                {
                    paisCodigoIso = localidadeDoCliente.Pais.CodigoIso;
                }
            }

            // Validação do NIF (código que você já tinha)
            if (!string.IsNullOrWhiteSpace(NumeroFiscal))
            {
                if (string.IsNullOrWhiteSpace(paisCodigoIso))
                {
                    // Não foi possível determinar o país, talvez adicionar um erro?
                }
                else if (!EuropeanNifValidator.ValidateNif(paisCodigoIso, NumeroFiscal))
                {
                    results.Add(new ValidationResult(
                        $"O NIF '{NumeroFiscal}' não é válido para o país '{paisCodigoIso}'.",
                        new[] { nameof(NumeroFiscal) }));
                }
            }

            // Nova validação para os Códigos Postais
            if (paisCodigoIso == "PT")
            {
                if (string.IsNullOrWhiteSpace(Cp4))
                {
                    results.Add(new ValidationResult("O CP (4 dígitos) é obrigatório para Portugal.", new[] { nameof(Cp4) }));
                }
                if (string.IsNullOrWhiteSpace(Cp3))
                {
                    results.Add(new ValidationResult("O CP (3 dígitos) é obrigatório para Portugal.", new[] { nameof(Cp3) }));
                }
                if (!string.IsNullOrWhiteSpace(CodigoPostalEstrangeiro))
                {
                    // Opcional: pode querer limpar este campo no controller ou aqui.
                }
            }
            else if (!string.IsNullOrWhiteSpace(paisCodigoIso)) // Se for estrangeiro mas não Portugal
            {
                if (string.IsNullOrWhiteSpace(CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("O Código Postal é obrigatório.", new[] { nameof(CodigoPostalEstrangeiro) }));
                }
            }

            foreach (var result in results)
            {
                yield return result;
            }
        }
    }
}