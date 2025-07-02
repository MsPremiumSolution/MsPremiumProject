// Ficheiro: ViewModels/ClienteCreateViewModel.cs

using Microsoft.AspNetCore.Mvc.Rendering;
using MSPremiumProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.ViewModels
{
    public class ClienteCreateViewModel
    {
        /// <summary>
        /// A instância do Cliente que está a ser criada ou editada.
        /// </summary>
        public Cliente Cliente { get; set; } = new Cliente();

        // --- DADOS PARA AS DROPDOWNS E LÓGICA DO FORMULÁRIO ---

        /// <summary>
        /// Guarda o ID do país selecionado.
        /// </summary>
        [Required(ErrorMessage = "É obrigatório selecionar um país.")]
        [Display(Name = "País")]
        public ulong SelectedPaisId { get; set; }

        /// <summary>
        /// Lista de países para a dropdown.
        /// </summary>
        public IEnumerable<SelectListItem> PaisesList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Guarda o texto do DISTRITO selecionado na dropdown (ex: "Lisboa").
        /// </summary>
        [Display(Name = "Distrito")]
        public string? SelectedRegiao { get; set; }

        /// <summary>
        /// Lista de distritos para a dropdown.
        /// **ESTA ERA A PROPRIEDADE EM FALTA.**
        /// </summary>
        public IEnumerable<SelectListItem> DistritosList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Guarda o texto da CIDADE/CONCELHO que o utilizador digita.
        /// </summary>
        [Required(ErrorMessage = "O nome da localidade (cidade/concelho) é obrigatório.")]
        [StringLength(200)]
        [Display(Name = "Localidade (Cidade/Concelho)")]
        public string NomeLocalidade { get; set; } = string.Empty;
    }
}