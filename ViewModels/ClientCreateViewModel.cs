// File: ViewModels/ClienteCreateViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using MSPremiumProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Adicionar este using

namespace MSPremiumProject.ViewModels
{
    public class ClienteCreateViewModel
    {
        public Cliente Cliente { get; set; } = new Cliente();

        // Para a dropdown de Países
        [Required(ErrorMessage = "É obrigatório selecionar um país.")]
        [Display(Name = "País")]
        public ulong SelectedPaisId { get; set; }
        public IEnumerable<SelectListItem> PaisesList { get; set; } = new List<SelectListItem>();

        // Para a dropdown de Regiões (só para Portugal)
        [Display(Name = "Região / Distrito")]
        public string? SelectedRegiao { get; set; }

        // NOVO: Campo de texto para o nome da localidade
        [Required(ErrorMessage = "O nome da localidade é obrigatório.")]
        [StringLength(200)]
        [Display(Name = "Localidade / Cidade")]
        public string NomeLocalidade { get; set; } = null!;

        // Campo para o código postal estrangeiro (opcional)
        [StringLength(20)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostalEstrangeiro { get; set; }
    }
}