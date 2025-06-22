using MSPremiumProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.ViewModels
{
    public class ClienteCreateViewModel
    {
        // Garante que Cliente é inicializado para evitar NullReferenceException na View
        public Cliente Cliente { get; set; } = new Cliente();

        [Display(Name = "Região")]
        // Não é [Required] aqui porque é apenas para controlo de UI,
        // a validação real será no Cliente.LocalidadeId
        public string? SelectedRegiao { get; set; }
        public List<SelectListItem> RegioesList { get; set; } = new List<SelectListItem>();

        // Este será populado via AJAX. O Cliente.LocalidadeId é o que será submetido.
        public List<SelectListItem> LocalidadesList { get; set; } = new List<SelectListItem>();
    }
}