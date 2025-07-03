// Ficheiro: Data/AppDbContext.cs - VERSÃO FINAL CORRIGIDA

using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Models; // Garante que o DbContext consegue encontrar todas as suas classes de modelo

namespace MSPremiumProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        #region DbSets - Entidades do Sistema e Configuração

        // Estas são as tabelas existentes que você quer manter
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Localidade> Localidades { get; set; }
        public DbSet<Pai> Paises { get; set; }
        public DbSet<Tipoobra> Tipoobras { get; set; }
        public DbSet<Tipotratamento> Tipotratamentos { get; set; }
        public DbSet<TipologiaConstrutiva> TipologiasConstrutivas { get; set; }

        #endregion

        #region DbSets - Entidade Principal da Proposta

        public DbSet<Proposta> Proposta { get; set; }

        #endregion

        #region DbSets - NOVA ESTRUTURA COMPLETA

        // Tabelas da nova estrutura de Qualidade do Ar
        public DbSet<Sintomatologia> Sintomatologia { get; set; }
        public DbSet<Higrometria> Higrometria { get; set; }
        public DbSet<DadosConstrutivos> DadosConstrutivos { get; set; }
        public DbSet<Janela> Janelas { get; set; }
        public DbSet<DadosGerais> DadosGerais { get; set; }
        public DbSet<Objetivos> Objetivos { get; set; }
        public DbSet<OrcamentoAr> OrcamentoAr { get; set; }
        public DbSet<QualidadeDoAr> QualidadeDoAr { get; set; }
        public DbSet<Volume> Volumes { get; set; } // O DbSet de Volumes, agora ligado a QualidadeDoAr

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define o Charset e Collation padrão para toda a base de dados
            modelBuilder
                .UseCollation("utf8mb4_unicode_ci")
                .HasCharSet("utf8mb4");

            // --- Configurações Específicas das Entidades ---

            // Relações da Entidade Proposta
            modelBuilder.Entity<Proposta>(entity =>
            {
                // Relação 1-para-1 com QualidadeDoAr
                entity.HasOne(p => p.QualidadeDoAr)
                      .WithOne(qa => qa.Proposta)
                      .HasForeignKey<Proposta>(p => p.QualidadeDoArId)
                      .OnDelete(DeleteBehavior.SetNull); // Se apagar QualidadeDoAr, o ID na Proposta fica nulo
            });

            // Relações da Entidade Utilizador
            modelBuilder.Entity<Utilizador>(entity =>
            {
                // Relação com Role
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Utilizadors)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Relações da Entidade PasswordResetToken
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                // Relação com Utilizador
                entity.HasOne(prt => prt.Utilizador)
                      .WithMany(u => u.PasswordResetTokens)
                      .HasForeignKey(prt => prt.UtilizadorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Relações da Entidade QualidadeDoAr (essencial para definir a cascata)
            modelBuilder.Entity<QualidadeDoAr>(entity => {
                // Relação 1-para-1 com DadosGerais
                entity.HasOne(qa => qa.DadosGerais)
                      .WithOne(dg => dg.QualidadeDoAr)
                      .HasForeignKey<QualidadeDoAr>(qa => qa.DadosGeraisId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relação 1-para-1 com Objetivos
                entity.HasOne(qa => qa.Objetivos)
                      .WithOne(o => o.QualidadeDoAr)
                      .HasForeignKey<QualidadeDoAr>(qa => qa.ObjetivosId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relação 1-para-1 com OrcamentoAr
                entity.HasOne(qa => qa.OrcamentoAr)
                      .WithOne(oa => oa.QualidadeDoAr)
                      .HasForeignKey<QualidadeDoAr>(qa => qa.OrcamentoArId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}