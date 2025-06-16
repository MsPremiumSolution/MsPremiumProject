// File: Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Models;

namespace MSPremiumProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Calculovolume> Calculovolumes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Construtivo> Construtivos { get; set; }
        public DbSet<Dadosgeral> Dadosgerals { get; set; }
        public DbSet<Higrometrium> Higrometria { get; set; }
        public DbSet<Localidade> Localidades { get; set; }
        public DbSet<Pai> Paises { get; set; }
        public DbSet<Propostum> Proposta { get; set; }
        public DbSet<Qualidadear> Qualidadears { get; set; }
        public DbSet<Sintoma> Sintomas { get; set; }
        public DbSet<Tipojanela> Tipojanelas { get; set; }
        public DbSet<Tipoobra> Tipoobras { get; set; }
        public DbSet<Tipotratamento> Tipotratamentos { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Volume> Volumes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .UseCollation("utf8mb4_unicode_ci")
                .HasCharSet("utf8mb4");

            // --- Configurações das Entidades ---
            // Certifique-se de que estas configurações correspondem ao seu banco de dados
            // e aos seus modelos C#. Os nomes de tabela devem ser os que existem no BD.

            modelBuilder.Entity<Calculovolume>(entity =>
            {
                entity.ToTable("calculovolume"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações conforme necessário
                // Ex: entity.HasKey(e => e.CalculoVolumeId);
                //     entity.Property(e => e.CalculoVolumeId).HasColumnName("CalculoVolumeID"); // Se o nome da coluna for diferente
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("cliente"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Construtivo>(entity =>
            {
                entity.ToTable("construtivo"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Dadosgeral>(entity =>
            {
                entity.ToTable("dadosgeral"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Higrometrium>(entity => // Classe C# é Higrometrium
            {
                entity.ToTable("higrometria"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Localidade>(entity =>
            {
                entity.ToTable("localidade"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
                // Exemplo de relação com 'pais' (tabela pai):
                // entity.HasOne(l => l.Pai) // Propriedade de navegação em Localidade.cs
                //       .WithMany(p => p.Localidades) // Coleção em Pai.cs
                //       .HasForeignKey(l => l.PaisId) // FK em Localidade.cs
                //       .HasPrincipalKey(p => p.PaisId) // PK em Pai.cs
                //       .OnDelete(DeleteBehavior.Restrict); // Ou o que for apropriado
            });

            modelBuilder.Entity<Pai>(entity => // Classe C# é Pai
            {
                entity.ToTable("pais"); // Nome da tabela no BD
                entity.HasKey(e => e.PaisId);
                entity.Property(e => e.PaisId)
                      .HasColumnName("PaisID") // Nome da coluna PK na tabela 'pais'
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.NomePais)
                      .HasColumnName("NomePais")
                      .HasMaxLength(255)
                      .IsRequired();
            });

            modelBuilder.Entity<Propostum>(entity => // Classe C# é Propostum
            {
                entity.ToTable("proposta"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Qualidadear>(entity =>
            {
                entity.ToTable("qualidadear"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role"); // Nome da tabela no BD
                entity.HasKey(e => e.RoleId); // PK da classe Role
                entity.Property(e => e.RoleId).HasColumnName("RoleID"); // Coluna PK na tabela 'role'
                entity.Property(e => e.Nome).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descricao).HasMaxLength(255).IsRequired(false);
            });

            modelBuilder.Entity<Sintoma>(entity =>
            {
                entity.ToTable("sintoma"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Tipojanela>(entity =>
            {
                entity.ToTable("tipojanela"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });
            modelBuilder.Entity<Tipoobra>(entity =>
            {
                entity.ToTable("tipoobra"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Tipotratamento>(entity =>
            {
                entity.ToTable("tipotratamento"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.ToTable("utilizador"); // Nome da tabela no BD
                entity.HasKey(e => e.UtilizadorId); // PK da classe Utilizador
                entity.Property(e => e.UtilizadorId).HasColumnName("UtilizadorID"); // Coluna PK na tabela 'utilizador'
                entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Login).HasMaxLength(255).IsRequired(); // Aumentado para Email
                entity.Property(e => e.Pwp).HasColumnName("PWP").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Dtnascimento).IsRequired();
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.Login).HasMaxLength(255).IsRequired(); // Assumindo que Login é o Email, esta pode ser redundante ou para um email secundário. Se Login é o email, remova esta.
                entity.Property(e => e.RoleId).HasColumnName("RoleID"); // Coluna FK na tabela 'utilizador'

                // Relação com Role
                entity.HasOne(d => d.Role) // Propriedade de navegação Role em Utilizador.cs
                      .WithMany(p => p.Utilizadors) // Coleção Utilizadors em Role.cs
                      .HasForeignKey(d => d.RoleId) // Propriedade FK RoleId em Utilizador.cs
                      .HasPrincipalKey(p => p.RoleId) // Propriedade PK RoleId em Role.cs
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Volume>(entity =>
            {
                entity.ToTable("volume"); // Nome da tabela no BD
                // Configure .HasKey(), .Property() e relações
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetToken"); // Nome da nova tabela
                entity.HasKey(t => t.Id);
                entity.Property(t => t.TokenValue).HasMaxLength(256).IsRequired();
                entity.HasIndex(t => t.TokenValue).IsUnique();
                entity.Property(t => t.ExpirationDate).IsRequired();
                entity.Property(t => t.IsUsed).IsRequired();
                entity.Property(t => t.UtilizadorId).HasColumnName("UtilizadorID"); // Nome da coluna FK na tabela PasswordResetToken

                entity.HasOne(prt => prt.Utilizador)              // Navegação de PasswordResetToken para Utilizador
                      .WithMany(u => u.PasswordResetTokens)       // ***** USA A COLEÇÃO EM Utilizador.cs *****
                      .HasForeignKey(prt => prt.UtilizadorId)     // Propriedade FK 'UtilizadorId' em PasswordResetToken.cs
                                                                  // que mapeia para a coluna 'UtilizadorID' na tabela PasswordResetToken
                      .HasPrincipalKey(u => u.UtilizadorId)       // Propriedade PK 'UtilizadorId' em Utilizador.cs
                                                                  // que mapeia para a coluna 'UtilizadorID' na tabela 'utilizador'
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}