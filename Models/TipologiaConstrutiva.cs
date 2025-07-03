using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models
{
    [Table("TipologiasConstrutivas")] // Nome da tabela na base de dados
    public class TipologiaConstrutiva
    {
        [Key]
        public ulong TipologiaConstrutivaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(255)]
        public string? ImagemUrl { get; set; }


        // Propriedade de navegação inversa: Uma tipologia pode estar em muitas propostas
        public virtual ICollection<Proposta> Propostas { get; set; } = new List<Proposta>();
    }
}