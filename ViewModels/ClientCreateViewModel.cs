// File: ViewModels/ClienteCreateViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using MSPremiumProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.ViewModels
{
    public class ClienteCreateViewModel
    {
        public Cliente Cliente { get; set; } = new Cliente();

        // Para a dropdown de Países
        public ulong SelectedPaisId { get; set; }
        public IEnumerable<SelectListItem> PaisesList { get; set; } = new List<SelectListItem>();

        // Para a dropdown de Regiões (será preenchida com AJAX)
        public string? SelectedRegiao { get; set; }

        // Novo campo para a localidade, que agora é uma textbox
        [Required(ErrorMessage = "A localidade é obrigatória.")]
        [StringLength(200)]
        public string NomeLocalidade { get; set; }

        // Novo campo para o código postal estrangeiro
        [StringLength(20)]
        public string? CodigoPostalEstrangeiro { get; set; }
    }
}