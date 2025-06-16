// File: ViewModels/ForgotPasswordViewModel.cs

using System.ComponentModel.DataAnnotations; // Necessário para Data Annotations como [Required]

namespace MSPremiumProject.ViewModels // Certifique-se que este namespace corresponde à sua estrutura de pastas
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "O campo Login é obrigatório.")]
        [Display(Name = "Login")] // Como o campo será exibido em labels, se usar asp-label-for
        [StringLength(100, ErrorMessage = "O login deve ter no máximo {1} caracteres.")] // Ajuste o tamanho máximo se necessário
        public string Login { get; set; }
    }
}