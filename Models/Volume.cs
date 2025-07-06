// File: MSPremiumProject/Models/Volume.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Volumes")]
public partial class Volume
{
    [Key]
    public ulong Id { get; set; }

    [Required]
    public ulong QualidadeDoArId { get; set; } // Chave estrangeira para QualidadeDoAr

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Altura { get; set; }

    public virtual QualidadeDoAr QualidadeDoAr { get; set; }
    public virtual ICollection<Medida> Medidas { get; set; } = new List<Medida>();
}