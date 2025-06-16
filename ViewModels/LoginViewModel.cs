// ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O email ou login é obrigatório.")]
        [Display(Name = "Email ou Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A password é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; } // Para cookies persistentes
    }
}