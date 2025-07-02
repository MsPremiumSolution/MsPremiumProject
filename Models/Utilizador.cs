// Ficheiro: Models/Utilizador.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models
{
    [Table("Utilizadores")]
    public partial class Utilizador
    {
        [Key]
        [Column("UtilizadorID")]
        public ulong UtilizadorId { get; set; }

        [Required(ErrorMessage = "O Role é obrigatório.")]
        [Column("RoleID")]
        [Display(Name = "Perfil de Utilizador")]
        public ulong RoleId { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = null!;

        // --- NOVO CAMPO EMPRESA ---
        [StringLength(150, ErrorMessage = "O Nome da Empresa deve ter no máximo 150 caracteres.")]
        [Display(Name = "Empresa (Opcional)")]
        public string? Empresa { get; set; } // O '?' torna o campo opcional (nullable)

        [Required(ErrorMessage = "O Login (Email) é obrigatório.")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "O formato do Login (Email) é inválido.")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "A Password é obrigatória.")]
        [Column("PWP")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Pwp { get; set; } = null!;

        [Required(ErrorMessage = "A Data de Nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime Dtnascimento { get; set; }

        public bool Activo { get; set; } = true;

        // --- Propriedades de Navegação ---
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Proposta> Proposta { get; set; }
        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; }

        // Construtor
        public Utilizador()
        {
            Proposta = new HashSet<Proposta>();
            PasswordResetTokens = new HashSet<PasswordResetToken>();
        }
    }
}