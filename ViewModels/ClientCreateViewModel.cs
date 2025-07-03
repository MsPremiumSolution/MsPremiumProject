using Microsoft.AspNetCore.Mvc.Rendering;
using MSPremiumProject.Models;
using MSPremiumProject.Data;
using MSPremiumProject.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MSPremiumProject.ViewModels
{
    public class ClienteCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Os dados do cliente são obrigatórios.")]
        public Cliente Cliente { get; set; } = new Cliente();

        [Required(ErrorMessage = "O país é obrigatório.")]
        [Display(Name = "País")]
        public ulong SelectedPaisId { get; set; }

        public List<SelectListItem> PaisesList { get; set; } = new List<SelectListItem>();

        [Display(Name = "Região / Distrito")] // Este campo é sempre populado pelo AJAX se houver dados
        public string? SelectedRegiao { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            var logger = validationContext.GetService<ILogger<ClienteCreateViewModel>>();

            if (dbContext == null)
            {
                logger?.LogError("Validate: AppDbContext não pôde ser injetado no ValidationContext.");
                results.Add(new ValidationResult("Erro de configuração: Serviço de base de dados indisponível para validação."));
                yield break;
            }

            string? paisCodigoIso = null;
            if (SelectedPaisId > 0)
            {
                var pais = dbContext.Paises.AsNoTracking().FirstOrDefault(p => p.PaisId == this.SelectedPaisId);
                if (pais != null)
                {
                    paisCodigoIso = pais.CodigoIso;
                }
            }

            // Validação do NIF
            if (!string.IsNullOrWhiteSpace(Cliente.NumeroFiscal))
            {
                if (string.IsNullOrWhiteSpace(paisCodigoIso))
                {
                    results.Add(new ValidationResult(
                        "Não foi possível determinar o país para validar o NIF. Por favor, selecione um país válido.",
                        new[] { nameof(SelectedPaisId), nameof(Cliente.NumeroFiscal) }));
                }
                else if (!EuropeanNifValidator.ValidateNif(paisCodigoIso, Cliente.NumeroFiscal))
                {
                    results.Add(new ValidationResult(
                        $"O NIF '{Cliente.NumeroFiscal}' não é válido para o país '{paisCodigoIso}'.",
                        new[] { nameof(Cliente.NumeroFiscal) }));
                }
            }

            // Validação dos Códigos Postais e da Região/Localidade
            if (paisCodigoIso == "PT")
            {
                // Para Portugal, SelectedRegiao (Distrito/Região) é obrigatório
                if (string.IsNullOrWhiteSpace(SelectedRegiao))
                {
                    results.Add(new ValidationResult("Para Portugal, a Região / Distrito é obrigatória.", new[] { nameof(SelectedRegiao) }));
                }
                // Para Portugal, os CP (4 e 3 dígitos) são obrigatórios
                if (string.IsNullOrWhiteSpace(Cliente.Cp4))
                {
                    results.Add(new ValidationResult("O CP (4 dígitos) é obrigatório para Portugal.", new[] { nameof(Cliente.Cp4) }));
                }
                if (string.IsNullOrWhiteSpace(Cliente.Cp3))
                {
                    results.Add(new ValidationResult("O CP (3 dígitos) é obrigatório para Portugal.", new[] { nameof(Cliente.Cp3) }));
                }
                // O CodigoPostalEstrangeiro deve estar vazio para Portugal
                if (!string.IsNullOrWhiteSpace(Cliente.CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("Código Postal Estrangeiro não deve ser preenchido para Portugal.", new[] { nameof(Cliente.CodigoPostalEstrangeiro) }));
                }
            }
            else if (!string.IsNullOrWhiteSpace(paisCodigoIso)) // Se for estrangeiro (qualquer país com Código ISO que não PT)
            {
                // Para países estrangeiros, o Código Postal estrangeiro é obrigatório
                if (string.IsNullOrWhiteSpace(Cliente.CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("O Código Postal é obrigatório para países estrangeiros.", new[] { nameof(Cliente.CodigoPostalEstrangeiro) }));
                }
                // Os CP portugueses (4 e 3 dígitos) devem estar vazios para países estrangeiros
                if (!string.IsNullOrWhiteSpace(Cliente.Cp4) || !string.IsNullOrWhiteSpace(Cliente.Cp3))
                {
                    results.Add(new ValidationResult("Os campos CP (4 Dígitos) e CP (3 Dígitos) devem estar vazios para países estrangeiros.", new[] { nameof(Cliente.Cp4), nameof(Cliente.Cp3) }));
                }
                // SelectedRegiao é opcional para países estrangeiros (apenas sugestão), não validamos aqui.
            }
            else // Nenhum país selecionado (SelectedPaisId é 0 ou nulo)
            {
                // Se não há país selecionado, mas há NIF ou CP preenchidos, é um erro.
                if (!string.IsNullOrWhiteSpace(Cliente.NumeroFiscal))
                {
                    results.Add(new ValidationResult("Por favor, selecione um país para validar o NIF.", new[] { nameof(SelectedPaisId) }));
                }
                if (!string.IsNullOrWhiteSpace(Cliente.Cp4) || !string.IsNullOrWhiteSpace(Cliente.Cp3) || !string.IsNullOrWhiteSpace(Cliente.CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("Por favor, selecione um país para validar o Código Postal.", new[] { nameof(SelectedPaisId) }));
                }
            }

            foreach (var result in results)
            {
                yield return result;
            }
        }
    }
}