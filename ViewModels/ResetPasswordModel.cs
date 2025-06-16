// File: ViewModels/ResetPasswordViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.ViewModels // Certifique-se que este namespace corresponde à sua estrutura de pastas
{
    public class ResetPasswordViewModel
    {
        // Estes campos são geralmente passados como hidden fields no formulário,
        // preenchidos a partir dos parâmetros da URL (do link do email).
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "A nova password é obrigatória.")]
        [DataType(DataType.Password)] // Para que o input seja do tipo password
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [Display(Name = "Nova Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova password")]
        [Compare("Password", ErrorMessage = "A password e a password de confirmação não correspondem.")]
        public string ConfirmPassword { get; set; }
    }
}