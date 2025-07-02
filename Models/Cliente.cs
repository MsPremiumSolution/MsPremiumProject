// Ficheiro: Models/Cliente.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSPremiumProject.Data; // Necessário para aceder ao AppDbContext
using Microsoft.EntityFrameworkCore; // Necessário para o Include() e AsNoTracking()
using System.Linq; // Necessário para o FirstOrDefault()
using MSPremiumProject.Utils; // Supondo que o seu validador de NIF está aqui

namespace MSPremiumProject.Models
{
    [Table("Clientes")]
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

        [StringLength(150, ErrorMessage = "O Nome da Empresa deve ter no máximo 150 caracteres.")]
        [Display(Name = "Empresa (Opcional)")]
        public string? Empresa { get; set; } // Campo Empresa já incluído

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

        // --- CAMPO OBSERVAÇÕES CORRIGIDO ---
        [DataType(DataType.MultilineText)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; } // Apenas o '?' já o torna opcional (nullable)

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
        public DateOnly? Dtnascimento { get; set; } // Usar DateOnly é mais apropriado se não precisar da hora

        // --- Propriedades de Navegação ---
        [ForeignKey("LocalidadeId")]
        public virtual Localidade? LocalidadeNavigation { get; set; } // Nome corrigido para seguir convenção
        public virtual ICollection<Proposta> Proposta { get; set; } = new List<Proposta>();


        // --- Método de Validação Personalizada ---
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Obtém a instância do DbContext para fazer validações contra a base de dados
            var dbContext = (AppDbContext?)validationContext.GetService(typeof(AppDbContext));
            if (dbContext == null)
            {
                // Se não conseguirmos obter o contexto (ex: em testes unitários), não podemos validar.
                // Retornar sem erros é uma abordagem segura para não bloquear o fluxo.
                yield break;
            }

            // Descobre o código do país com base na LocalidadeId selecionada
            string? paisCodigoIso = null;
            if (this.LocalidadeId > 0)
            {
                var pais = dbContext.Localidades
                                    .Where(l => l.LocalidadeId == this.LocalidadeId)
                                    .Select(l => l.Pais.CodigoIso)
                                    .AsNoTracking()
                                    .FirstOrDefault();
                paisCodigoIso = pais;
            }

            // 1. Validação do NIF com base no país
            if (!string.IsNullOrWhiteSpace(NumeroFiscal))
            {
                if (string.IsNullOrWhiteSpace(paisCodigoIso))
                {
                    yield return new ValidationResult(
                        "Não foi possível validar o NIF porque a localidade (e o país) não foi selecionada.",
                        new[] { nameof(NumeroFiscal) });
                }
                else if (!EuropeanNifValidator.ValidateNif(paisCodigoIso, NumeroFiscal)) // Supondo que o seu validador existe
                {
                    yield return new ValidationResult(
                        $"O NIF '{NumeroFiscal}' não é válido para o país selecionado.",
                        new[] { nameof(NumeroFiscal) });
                }
            }

            // 2. Validação condicional do Código Postal
            if (paisCodigoIso == "PT")
            {
                // Se o país é Portugal, os campos Cp4 e Cp3 são obrigatórios
                if (string.IsNullOrWhiteSpace(this.Cp4))
                {
                    yield return new ValidationResult("O CP (4 dígitos) é obrigatório para Portugal.", new[] { nameof(Cp4) });
                }
                if (string.IsNullOrWhiteSpace(this.Cp3))
                {
                    yield return new ValidationResult("O CP (3 dígitos) é obrigatório para Portugal.", new[] { nameof(Cp3) });
                }
            }
            else if (!string.IsNullOrWhiteSpace(paisCodigoIso)) // Se for um país estrangeiro conhecido
            {
                // Se for estrangeiro, o CodigoPostalEstrangeiro é obrigatório
                if (string.IsNullOrWhiteSpace(this.CodigoPostalEstrangeiro))
                {
                    yield return new ValidationResult("O Código Postal é obrigatório para países estrangeiros.", new[] { nameof(CodigoPostalEstrangeiro) });
                }
            }
        }
    }
}