// File: Models/Cliente.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSPremiumProject.Utils;         // Para EuropeanNifValidator
using MSPremiumProject.Data;          // Para AppDbContext
using Microsoft.EntityFrameworkCore;  // Para Include()
using System.Linq;                    // Para FirstOrDefault()

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

        [Phone(ErrorMessage = "O formato do telefone é inválido.")]
        [StringLength(20, ErrorMessage = "O telefone não pode exceder 20 caracteres.")]
        [Display(Name = "Telefone 1")]
        public string? Telefone1 { get; set; }

        [Phone(ErrorMessage = "O formato do telefone é inválido.")]
        [StringLength(20, ErrorMessage = "O telefone não pode exceder 20 caracteres.")]
        [Display(Name = "Telefone 2")]
        public string? Telefone2 { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateOnly? Dtnascimento { get; set; }

        [ForeignKey("LocalidadeId")]
        public virtual Localidade? LocalidadeNavigation { get; set; }

        public virtual ICollection<Propostum> Proposta { get; set; } = new List<Propostum>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Validação do NIF se estiver preenchido
            if (!string.IsNullOrWhiteSpace(NumeroFiscal))
            {
                string? paisCodigoParaNif = null;
                var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;

                if (dbContext == null)
                {
                    results.Add(new ValidationResult("Erro interno: não foi possível validar o NIF.", new[] { nameof(NumeroFiscal) }));
                    return results;
                }

                if (LocalidadeId > 0)
                {
                    var localidadeDoCliente = dbContext.Localidades
                                                  .Include(l => l.Pais) // Crucial para aceder a Pais.CodigoIso
                                                  .AsNoTracking() // Boa prática para queries apenas de leitura
                                                  .FirstOrDefault(l => l.LocalidadeId == this.LocalidadeId);

                    if (localidadeDoCliente != null && localidadeDoCliente.Pais != null)
                    {
                        // Usa a propriedade CodigoIso do modelo Pai.
                        // Se o teu modelo Pai usa NomePais para o código ISO, altera aqui.
                        paisCodigoParaNif = localidadeDoCliente.Pais.CodigoIso;
                    }
                }

                if (string.IsNullOrWhiteSpace(paisCodigoParaNif))
                {
                    results.Add(new ValidationResult(
                        "Selecione uma localidade válida para que o país possa ser determinado para a validação do NIF.",
                        new[] { nameof(NumeroFiscal), nameof(LocalidadeId) }));
                }
                else
                {
                    if (!EuropeanNifValidator.ValidateNif(paisCodigoParaNif, NumeroFiscal))
                    {
                        // A mensagem de "país não suportado" virá do EuropeanNifValidator se ele retornar false por essa razão.
                        // Ou, podemos ter uma mensagem mais específica aqui se ValidateNif retornar um código de erro.
                        // Por agora, a mensagem genérica de NIF inválido para o país.
                        results.Add(new ValidationResult(
                            $"O NIF '{NumeroFiscal}' não é válido ou não é suportado para o país '{paisCodigoParaNif}'.",
                            new[] { nameof(NumeroFiscal) }));
                    }
                }
            }

            // Adicionar outras validações a nível de classe aqui, se necessário
            // Exemplo: if (string.IsNullOrWhiteSpace(Telefone1) && string.IsNullOrWhiteSpace(Email))
            // {
            //     results.Add(new ValidationResult("É necessário fornecer pelo menos um Telefone ou Email.", 
            //                  new[] { nameof(Telefone1), nameof(Email) }));
            // }

            return results;
        }
    }
}