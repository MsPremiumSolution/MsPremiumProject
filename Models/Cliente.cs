// File: Models/Cliente.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Adicionar para [ForeignKey]

namespace MSPremiumProject.Models
{
    public partial class Cliente
    {
        [Key]
        public ulong ClienteId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [StringLength(100)]
        public string? Apelido { get; set; } // Permitir nulo se for opcional

        [Required(ErrorMessage = "A morada é obrigatória.")]
        [StringLength(255)]
        public string Morada { get; set; } = null!;

        [Required(ErrorMessage = "O CP4 é obrigatório.")]
        [StringLength(4)]
        public string Cp4 { get; set; } = null!;

        [Required(ErrorMessage = "O CP3 é obrigatório.")]
        [StringLength(3)]
        public string Cp3 { get; set; } = null!;

        // Chave estrangeira para a entidade Localidade
        [Required(ErrorMessage = "A localidade é obrigatória.")]
        public ulong LocalidadeId { get; set; }

        // A LINHA "public string Localidade { get; set; } = null!;" FOI REMOVIDA DAQUI.

        [StringLength(9)] // Assumindo NIF com 9 dígitos
        public string? NumeroFiscal { get; set; } // Permitir nulo se for opcional

        public string? Observacoes { get; set; } // Permitir nulo

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; } // Permitir nulo

        // Considerar string para telefones para flexibilidade de formato (+, espaços, etc.)
        public string? Telefone1 { get; set; } // Se Telefone1 é long, não pode ser null por defeito. Se for opcional, use long? ou string?

        public string? Telefone2 { get; set; } // Idem para Telefone2

        // DateOnly é bom, mas DateTime? é mais universalmente suportado por ORMs/BDs se houver problemas.
        // Se for obrigatório, use DateOnly. Se opcional, DateOnly?
        public DateOnly? Dtnascimento { get; set; } // Tornar nullable se for opcional

        // Propriedade de navegação para a entidade Localidade
        // Renomear "LocalidadeNavigation" para "Localidade" é uma convenção comum.
        // O [ForeignKey("LocalidadeId")] pode ser adicionado acima de LocalidadeId
        // ou a relação configurada via Fluent API no DbContext.
        // Em Cliente.cs (temporariamente)
        [ForeignKey("LocalidadeId")] // Ou como estivesse antes
        public virtual Localidade? LocalidadeNavigation { get; set; } // Volta ao nome antigo // Renomeado e tornado nullable

        public virtual ICollection<Propostum> Proposta { get; set; } = new List<Propostum>();
    }
}