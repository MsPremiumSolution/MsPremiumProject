using Microsoft.AspNetCore.Mvc.Rendering;
using MSPremiumProject.Models;
using MSPremiumProject.Data; // Para AppDbContext
using MSPremiumProject.Utils; // Para EuropeanNifValidator
using Microsoft.EntityFrameworkCore; // Para Include, AsNoTracking, FirstOrDefaultAsync
using System.ComponentModel.DataAnnotations; // Para IValidatableObject, ValidationResult
using System.Linq; // Para FirstOrDefault

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

        [Display(Name = "Região / Distrito")]
        public string? SelectedRegiao { get; set; } // Usado para Portugal

        // Não precisamos de NomeLocalidade na ViewModel agora, pois Cliente.NomeLocalidadeTexto trata disso.
        // O campo no formulário estará ligado diretamente a Cliente.NomeLocalidadeTexto

        // Implementação do IValidatableObject para validações complexas
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            if (dbContext == null)
            {
                // Se não conseguir obter o contexto da BD, não é possível validar NIF/CP por país.
                // Pode adicionar um erro ou simplesmente retornar, dependendo da criticidade.
                // Para produção, provavelmente lançaria uma exceção ou registaria um erro grave.
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

            // Validação dos Códigos Postais e da Localidade de Texto
            if (paisCodigoIso == "PT")
            {
                if (string.IsNullOrWhiteSpace(SelectedRegiao))
                {
                    results.Add(new ValidationResult("Para Portugal, a Região / Distrito é obrigatória.", new[] { nameof(SelectedRegiao) }));
                }
                if (string.IsNullOrWhiteSpace(Cliente.Cp4))
                {
                    results.Add(new ValidationResult("O CP (4 dígitos) é obrigatório para Portugal.", new[] { nameof(Cliente.Cp4) }));
                }
                if (string.IsNullOrWhiteSpace(Cliente.Cp3))
                {
                    results.Add(new ValidationResult("O CP (3 dígitos) é obrigatório para Portugal.", new[] { nameof(Cliente.Cp3) }));
                }
                // O CodigoPostalEstrangeiro deve estar vazio se for Portugal
                if (!string.IsNullOrWhiteSpace(Cliente.CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("Código Postal Estrangeiro não deve ser preenchido para Portugal.", new[] { nameof(Cliente.CodigoPostalEstrangeiro) }));
                }
                // A localidade de texto deve ser preenchida com a região selecionada para Portugal
                if (!string.IsNullOrWhiteSpace(SelectedRegiao) && Cliente.NomeLocalidadeTexto != SelectedRegiao)
                {
                    // Correção automática ou erro, dependendo da sua regra.
                    // Por exemplo: Cliente.NomeLocalidadeTexto = SelectedRegiao; (no controller)
                    // Ou um erro para indicar inconsistência.
                }
            }
            else if (!string.IsNullOrWhiteSpace(paisCodigoIso)) // Se for estrangeiro
            {
                if (string.IsNullOrWhiteSpace(Cliente.CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("O Código Postal é obrigatório para países estrangeiros.", new[] { nameof(Cliente.CodigoPostalEstrangeiro) }));
                }
                // Os CP portugueses devem estar vazios
                if (!string.IsNullOrWhiteSpace(Cliente.Cp4) || !string.IsNullOrWhiteSpace(Cliente.Cp3))
                {
                    results.Add(new ValidationResult("Os campos CP (4 Dígitos) e CP (3 Dígitos) devem estar vazios para países estrangeiros.", new[] { nameof(Cliente.Cp4), nameof(Cliente.Cp3) }));
                }
                // A localidade de texto é obrigatória para países estrangeiros
                if (string.IsNullOrWhiteSpace(Cliente.NomeLocalidadeTexto))
                {
                    results.Add(new ValidationResult("A Localidade é obrigatória para países estrangeiros.", new[] { nameof(Cliente.NomeLocalidadeTexto) }));
                }
                // O dropdown de região não é aplicável para estrangeiros, mas o campo SelectedRegiao
                // pode ter um valor se o utilizador trocou de Portugal para outro país e não foi limpo.
                if (!string.IsNullOrWhiteSpace(SelectedRegiao))
                {
                    // Você pode optar por adicionar um erro ou ignorar, dependendo da necessidade.
                    // Para evitar confusão, vamos assumir que o JS irá limpar isso.
                }
            }
            else // Nenhum país selecionado
            {
                if (!string.IsNullOrWhiteSpace(Cliente.NumeroFiscal))
                {
                    results.Add(new ValidationResult("Por favor, selecione um país para validar o NIF.", new[] { nameof(SelectedPaisId) }));
                }
                if (!string.IsNullOrWhiteSpace(Cliente.Cp4) || !string.IsNullOrWhiteSpace(Cliente.Cp3) || !string.IsNullOrWhiteSpace(Cliente.CodigoPostalEstrangeiro))
                {
                    results.Add(new ValidationResult("Por favor, selecione um país para validar o Código Postal.", new[] { nameof(SelectedPaisId) }));
                }
                if (string.IsNullOrWhiteSpace(Cliente.NomeLocalidadeTexto))
                {
                    results.Add(new ValidationResult("A Localidade é obrigatória.", new[] { nameof(Cliente.NomeLocalidadeTexto) }));
                }
            }


            foreach (var result in results)
            {
                yield return result;
            }
        }
    }
}