// File: Models/PasswordResetToken.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models // Certifique-se que este namespace corresponde à sua estrutura
{
    public class PasswordResetToken
    {
        [Key] // Marca esta propriedade como a chave primária
        public int Id { get; set; }

        [Required]
        public ulong UtilizadorId { get; set; } // Chave estrangeira para a sua tabela de Utilizadores

        // Propriedade de navegação para o Utilizador.
        // O nome "Utilizador" aqui deve corresponder ao nome da sua classe de modelo de utilizador.
        // A propriedade "UtilizadorId" acima será a chave estrangeira.
        [ForeignKey("UtilizadorId")]
        public virtual Utilizador Utilizador { get; set; }

        [Required]
        [StringLength(256)] // Um tamanho razoável para um token em Base64 ou similar
        public string TokenValue { get; set; } // O valor do token em si

        [Required]
        public DateTime ExpirationDate { get; set; } // Data e hora de expiração do token

        public bool IsUsed { get; set; } = false; // Indica se o token já foi utilizado (default é false)
    }
}