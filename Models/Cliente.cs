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

        // CAMPO 'Empresa' adicionado na resposta anterior, mantido.
        [StringLength(255, ErrorMessage = "O nome da empresa não pode exceder 255 caracteres.")]
        [Display(Name = "Empresa")]
        public string? Empresa { get; set; }

        [Required(ErrorMessage = "A morada é obrigatória.")]
        [StringLength(255, ErrorMessage = "A morada não pode exceder 255 caracteres.")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = null!;

        // CAMPO NOVO: Para guardar o texto da localidade diretamente no Cliente
        [StringLength(100, ErrorMessage = "A localidade não pode exceder 100 caracteres.")]
        [Display(Name = "Localidade (Texto)")] // Mudei o nome de Display para ser claro
        public string? NomeLocalidadeTexto { get; set; } // Este campo vai guardar o texto que o utilizador digita

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
        // usada para obter o país para validação de NIF/CP.
        [Required(ErrorMessage = "A localidade é obrigatória.")] // A LocalidadeId continua sendo obrigatória
        [Display(Name = "ID da Localidade Associada")] // Nome para diferenciar do campo texto
        public ulong LocalidadeId { get; set; }

        [Display(Name = "Número Fiscal (NIF)")]
        [StringLength(50, ErrorMessage = "O NIF não pode exceder 50 caracteres.")]
        public string? NumeroFiscal { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; } // Já estava como string? conforme solicitado

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            if (dbContext == null)
            {
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
                    // Se não conseguir determinar o país, o NIF não pode ser validado
                    results.Add(new ValidationResult(
                        "Não foi possível validar o NIF. Verifique a localidade e o país.",
                        new[] { nameof(NumeroFiscal) }));
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
                // O CodigoPostalEstrangeiro deve estar vazio se for Portugal
                if (!string.IsNullOrWhiteSpace(CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("Código Postal Estrangeiro não deve ser preenchido para Portugal.", new[] { nameof(CodigoPostalEstrangeiro) }));
                }
            }
            else if (!string.IsNullOrWhiteSpace(paisCodigoIso)) // Se for estrangeiro mas não Portugal
            {
                if (string.IsNullOrWhiteSpace(CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("O Código Postal é obrigatório para países estrangeiros.", new[] { nameof(CodigoPostalEstrangeiro) }));
                }
                // Os CP portugueses devem estar vazios
                if (!string.IsNullOrWhiteSpace(Cp4) || !string.IsNullOrWhiteSpace(Cp3))
                {
                    results.Add(new ValidationResult("Os campos CP (4 Dígitos) e CP (3 Dígitos) devem estar vazios para países estrangeiros.", new[] { nameof(Cp4), nameof(Cp3) }));
                }
            }

            // Validação para garantir que NomeLocalidadeTexto não está vazio se o País for PT (ou outro que o exija)
            // Esta validação pode ser ajustada conforme a sua regra de negócio para este novo campo.
            if (string.IsNullOrWhiteSpace(NomeLocalidadeTexto))
            {
                // Você pode adicionar uma validação aqui se o campo de texto da localidade for sempre obrigatório,
                // independentemente da LocalidadeId que é obrigatória.
                // Por enquanto, vou deixá-lo opcional no modelo e sem Required para não conflitar com LocalidadeId.
                // Se quiser que seja obrigatório, adicione [Required] no campo NomeLocalidadeTexto em Cliente.cs
            }


            foreach (var result in results)
            {
                yield return result;
            }
        }
    }
}