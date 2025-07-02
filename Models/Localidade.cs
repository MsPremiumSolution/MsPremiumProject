// File: Models/Localidade.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ... outros usings

namespace MSPremiumProject.Models
{
    public partial class Localidade
    {
        [Key]
        public ulong LocalidadeId { get; set; }
        public ulong PaisId { get; set; }

        // ESTA É A ÚNICA PROPRIEDADE DE NOME AGORA
        [Required(ErrorMessage = "O nome do Distrito é obrigatório.")]
        [StringLength(200)]
        [Display(Name = "Nome da Localidade")]
        public string Regiao { get; set; } = null!; // Antiga 'Regiao'

        // A propriedade 'NomeLocalidade' foi REMOVIDA
        // A propriedade 'Regiao' foi RENOMEADA para 'Nome'

        public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
        [ForeignKey("PaisId")]
        public virtual Pai Pais { get; set; } = null!;
    }
}