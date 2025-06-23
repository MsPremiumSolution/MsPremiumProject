using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.Models;

public partial class Pai
{
    [Key]
    public ulong PaisId { get; set; }
    [Required]
    [StringLength(255)]
    public string NomePais { get; set; } = null!;
    [Required(ErrorMessage = "O código ISO do país é obrigatório.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O código ISO deve ter 2 caracteres.")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "O código ISO deve consistir em 2 letras maiúsculas.")]
    [Display(Name = "Código ISO")]
    public string CodigoIso { get; set; } = null!; // Ex: "PT", "ES", "FR"

    public virtual ICollection<Localidade> Localidades { get; set; } = new List<Localidade>();
}
