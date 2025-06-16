// File: Models/Utilizador.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models
{
    public partial class Utilizador
    {
        [Key]
        [Column("UtilizadorID")]
        public ulong UtilizadorId { get; set; }

        [Required(ErrorMessage = "O Role é obrigatório.")]
        [Column("RoleID")]
        public ulong RoleId { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O Login (Email) é obrigatório.")]
        [StringLength(255, ErrorMessage = "O Login (Email) deve ter no máximo 255 caracteres.")] // Aumentei para acomodar emails mais longos
        [EmailAddress(ErrorMessage = "O formato do Login (Email) é inválido.")] // Valida se é um email
        public string Login { get; set; } = null!; // ESTE CAMPO É O EMAIL

        [Required(ErrorMessage = "A Password é obrigatória.")]
        [Column("PWP")]
        [StringLength(255)] // Para o hash da password
        public string Pwp { get; set; } = null!;

        [Required(ErrorMessage = "A Data de Nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime Dtnascimento { get; set; }

        public bool Activo { get; set; } = true;

        // --- Propriedades de Navegação ---

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        public virtual ICollection<Propostum> Proposta { get; set; }

        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; }

        // Construtor
        public Utilizador()
        {
            Proposta = new HashSet<Propostum>();
            PasswordResetTokens = new HashSet<PasswordResetToken>();
        }
    }
}