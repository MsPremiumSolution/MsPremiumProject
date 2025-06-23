// File: Models/Cliente.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSPremiumProject.Utils;
using MSPremiumProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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

        [Required(ErrorMessage = "O CP4 é obrigatório.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "O CP4 deve ter 4 dígitos.")]
        [StringLength(4)]
        [Display(Name = "CP (4 Dígitos)")]
        public string Cp4 { get; set; } = null!;

        [Required(ErrorMessage = "O CP3 é obrigatório.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "O CP3 deve ter 3 dígitos.")]
        [StringLength(3)]
        [Display(Name = "CP (3 Dígitos)")]
        public string Cp3 { get; set; } = null!;

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

        // ALTERADO AQUI para long? (nullable long)
        [Display(Name = "Telefone 1")]
        public long? Telefone1 { get; set; }

        // ALTERADO AQUI para long? (nullable long)
        [Display(Name = "Telefone 2")]
        public long? Telefone2 { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateOnly? Dtnascimento { get; set; } // BD é `date NOT NULL`, C# é `DateOnly?`. Inconsistência de nullability.

        [ForeignKey("LocalidadeId")]
        public virtual Localidade? LocalidadeNavigation { get; set; }

        public virtual ICollection<Propostum> Proposta { get; set; } = new List<Propostum>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            // ... (lógica de validação do NIF como antes) ...
            if (!string.IsNullOrWhiteSpace(NumeroFiscal))
            {
                string? paisCodigoParaNif = null;
                var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;

                if (dbContext == null) { /* ... erro ... */ return results; }

                if (LocalidadeId > 0)
                {
                    var localidadeDoCliente = dbContext.Localidades
                                                  .Include(l => l.Pais)
                                                  .AsNoTracking()
                                                  .FirstOrDefault(l => l.LocalidadeId == this.LocalidadeId);

                    if (localidadeDoCliente != null && localidadeDoCliente.Pais != null)
                    {
                        paisCodigoParaNif = localidadeDoCliente.Pais.CodigoIso;
                    }
                }

                if (string.IsNullOrWhiteSpace(paisCodigoParaNif)) { /* ... erro ... */ }
                else
                {
                    if (!EuropeanNifValidator.ValidateNif(paisCodigoParaNif, NumeroFiscal))
                    {
                        results.Add(new ValidationResult(
                            $"O NIF '{NumeroFiscal}' não é válido ou não é suportado para o país '{paisCodigoParaNif}'.",
                            new[] { nameof(NumeroFiscal) }));
                    }
                }
            }
            return results;
        }
}

    }