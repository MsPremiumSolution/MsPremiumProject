// MSPremiumProject/Models/Role.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Adicionar para DataAnnotations
using System.ComponentModel.DataAnnotations.Schema; // Adicionar para Table attribute

namespace MSPremiumProject.Models
{
    // [Table("Role")] // Já tem isto no OnModelCreating, mas pode adicionar aqui também para clareza
    public partial class Role
    {
        [Key] // Boa prática adicionar explicitamente, embora o EF Core possa inferir por convenção (NomeDaClasse + Id)
        [Column("RoleID")] // Se o nome da coluna na BD for diferente de RoleId
        public ulong RoleId { get; set; }

        [Required(ErrorMessage = "O nome do Role é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do Role deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; } = null!;

        // Descricao pode ser opcional, dependendo dos seus requisitos
        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
        public string Descricao { get; set; } = null!; // Se pode ser nulo na BD, pode remover `= null!` e tornar a string anulável `string?`

        // A propriedade de navegação está correta para a relação um-para-muitos
        // Um Role tem muitos Utilizadores
        public virtual ICollection<Utilizador> Utilizadors { get; set; } = new List<Utilizador>();
    }
}