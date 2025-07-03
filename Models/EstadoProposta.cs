// Models/EstadoProposta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models
{
    [Table("EstadoProposta")]
    public class EstadoProposta
    {
        [Key]
        public int EstadoPropostaId { get; set; } // Pode ser int ou byte, não precisa de ser ulong

        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = null!;
    }
}