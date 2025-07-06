// File: MSPremiumProject/Models/Medida.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Medidas")]
public partial class Medida
{
    [Key]
    public ulong Id { get; set; }

    [Required]
    public ulong VolumeId { get; set; } // Chave estrangeira para Volume

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Largura { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Comprimento { get; set; }

    public virtual Volume Volume { get; set; }
}