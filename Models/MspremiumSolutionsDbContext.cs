using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MSPremiumProject.Models;

public partial class MspremiumSolutionsDbContext : DbContext
{
    public MspremiumSolutionsDbContext()
    {
    }

    public MspremiumSolutionsDbContext(DbContextOptions<MspremiumSolutionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Calculovolume> Calculovolumes { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Construtivo> Construtivos { get; set; }

    public virtual DbSet<Dadosgeral> Dadosgerals { get; set; }

    public virtual DbSet<Higrometrium> Higrometria { get; set; }

    public virtual DbSet<Localidade> Localidades { get; set; }

    public virtual DbSet<Pai> Pais { get; set; }

    public virtual DbSet<Propostum> Proposta { get; set; }

    public virtual DbSet<Qualidadear> Qualidadears { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sintoma> Sintomas { get; set; }

    public virtual DbSet<Tipojanela> Tipojanelas { get; set; }

    public virtual DbSet<Tipoobra> Tipoobras { get; set; }

    public virtual DbSet<Tipotratamento> Tipotratamentos { get; set; }

    public virtual DbSet<Utilizador> Utilizadors { get; set; }

    public virtual DbSet<Volume> Volumes { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Calculovolume>(entity =>
        {
            entity.HasKey(e => e.CalculoVolumeId).HasName("PRIMARY");

            entity.ToTable("calculovolume");

            entity.HasIndex(e => e.VolumeId, "calculovolume_volumeid_fk");

            entity.Property(e => e.CalculoVolumeId).HasColumnName("CalculoVolumeID");
            entity.Property(e => e.AlturaMetros).HasPrecision(10, 2);
            entity.Property(e => e.Comprimento).HasPrecision(10, 2);
            entity.Property(e => e.Estadia1).HasPrecision(10, 2);
            entity.Property(e => e.EstadiaDireta).HasPrecision(10, 2);
            entity.Property(e => e.Largura).HasPrecision(10, 2);
            entity.Property(e => e.VolumeId).HasColumnName("VolumeID");

            entity.HasOne(d => d.Volume).WithMany(p => p.Calculovolumes)
                .HasForeignKey(d => d.VolumeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("calculovolume_volumeid_fk");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PRIMARY");

            entity.ToTable("cliente");

            entity.HasIndex(e => e.LocalidadeId, "cliente_localidadeid_fk");

            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Apelido).HasMaxLength(255);
            entity.Property(e => e.Cp3)
                .HasMaxLength(255)
                .HasColumnName("CP3");
            entity.Property(e => e.Cp4)
                .HasMaxLength(255)
                .HasColumnName("CP4");
            entity.Property(e => e.Email).HasMaxLength(255);
           // entity.Property(e => e.Localidade).HasMaxLength(255);
            entity.Property(e => e.LocalidadeId).HasColumnName("LocalidadeID");
            entity.Property(e => e.Morada).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.NumeroFiscal).HasMaxLength(255);
            entity.Property(e => e.Observacoes).HasMaxLength(255);

            entity.HasOne(d => d.LocalidadeNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.LocalidadeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cliente_localidadeid_fk");
        });

        modelBuilder.Entity<Construtivo>(entity =>
        {
            entity.HasKey(e => e.ConstrutivoId).HasName("PRIMARY");

            entity.ToTable("construtivo");

            entity.HasIndex(e => e.PropostaId, "construtivo_propostaid_fk");

            entity.Property(e => e.ConstrutivoId).HasColumnName("ConstrutivoID");
            entity.Property(e => e.AnoConstrucao).HasPrecision(10, 2);
            entity.Property(e => e.Area).HasPrecision(10, 2);
            entity.Property(e => e.Data).HasColumnType("datetime");
            entity.Property(e => e.FechamentoCobertura).HasMaxLength(255);
            entity.Property(e => e.FechamentoCoberturaPosterior).HasMaxLength(255);
            entity.Property(e => e.FechamentoOrientacao).HasMaxLength(255);
            entity.Property(e => e.FechamentoTipoFachada).HasMaxLength(255);
            entity.Property(e => e.Localidade).HasMaxLength(255);
            entity.Property(e => e.Nandares)
                .HasPrecision(10, 2)
                .HasColumnName("NAndares");
            entity.Property(e => e.Nhabitantes)
                .HasPrecision(10, 2)
                .HasColumnName("NHabitantes");
            entity.Property(e => e.PropostaId).HasColumnName("PropostaID");

            entity.HasOne(d => d.Proposta).WithMany(p => p.Construtivos)
                .HasForeignKey(d => d.PropostaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("construtivo_propostaid_fk");
        });

        modelBuilder.Entity<Dadosgeral>(entity =>
        {
            entity.HasKey(e => e.DadosGeralId).HasName("PRIMARY");

            entity.ToTable("dadosgeral");

            entity.HasIndex(e => e.DgTipoJanelaId, "dadosgeral_dgtipojanelaid_fk");

            entity.HasIndex(e => e.PropostaId, "dadosgeral_propostaid_fk");

            entity.HasIndex(e => e.TipoObraId, "dadosgeral_tipoobraid_fk");

            entity.HasIndex(e => e.TipoTratamentoId, "dadosgeral_tipotratamentoid_fk");

            entity.Property(e => e.DadosGeralId).HasColumnName("DadosGeralID");
            entity.Property(e => e.DgCoberturaFposterior)
                .HasMaxLength(255)
                .HasColumnName("DG_CoberturaFPosterior");
            entity.Property(e => e.DgCoberturaFprincipal)
                .HasMaxLength(255)
                .HasColumnName("DG_CoberturaFPrincipal");
            entity.Property(e => e.DgIsolamentoAquecimento)
                .HasMaxLength(255)
                .HasColumnName("DG_IsolamentoAquecimento");
            entity.Property(e => e.DgIsolamentoCamera)
                .HasMaxLength(255)
                .HasColumnName("DG_IsolamentoCamera");
            entity.Property(e => e.DgIsolamentoInterno)
                .HasMaxLength(255)
                .HasColumnName("DG_IsolamentoInterno");
            entity.Property(e => e.DgOrientacao)
                .HasMaxLength(255)
                .HasColumnName("DG_Orientacao");
            entity.Property(e => e.DgTipoFachada)
                .HasMaxLength(255)
                .HasColumnName("DG_TipoFachada");
            entity.Property(e => e.DgTipoJanelaCaixasPersiana).HasColumnName("DG_TipoJanelaCaixasPersiana");
            entity.Property(e => e.DgTipoJanelaDuplas).HasColumnName("DG_TipoJanelaDuplas");
            entity.Property(e => e.DgTipoJanelaId).HasColumnName("DG_TipoJanelaID");
            entity.Property(e => e.DgTipoJanelaMaterial)
                .HasMaxLength(255)
                .HasColumnName("DG_TipoJanelaMaterial");
            entity.Property(e => e.DgTipoJanelaRpt).HasColumnName("DG_TipoJanelaRPT");
            entity.Property(e => e.DgTipoJanelaUnidades)
                .HasPrecision(10, 2)
                .HasColumnName("DG_TipoJanelaUnidades");
            entity.Property(e => e.DgTipoJanelaVidro)
                .HasMaxLength(255)
                .HasColumnName("DG_TipoJanelaVidro");
            entity.Property(e => e.DgTratamentoHidrofugacao).HasColumnName("DG_TratamentoHidrofugacao");
            entity.Property(e => e.PropostaId).HasColumnName("PropostaID");
            entity.Property(e => e.TipoObraId).HasColumnName("TipoObraID");
            entity.Property(e => e.TipoTratamentoId).HasColumnName("TipoTratamentoID");

            entity.HasOne(d => d.DgTipoJanela).WithMany(p => p.Dadosgerals)
                .HasForeignKey(d => d.DgTipoJanelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dadosgeral_dgtipojanelaid_fk");

            entity.HasOne(d => d.Proposta).WithMany(p => p.Dadosgerals)
                .HasForeignKey(d => d.PropostaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dadosgeral_propostaid_fk");

            entity.HasOne(d => d.TipoObra).WithMany(p => p.Dadosgerals)
                .HasForeignKey(d => d.TipoObraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dadosgeral_tipoobraid_fk");

            entity.HasOne(d => d.TipoTratamento).WithMany(p => p.Dadosgerals)
                .HasForeignKey(d => d.TipoTratamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dadosgeral_tipotratamentoid_fk");
        });

        modelBuilder.Entity<Higrometrium>(entity =>
        {
            entity.HasKey(e => e.HigrometriaId).HasName("PRIMARY");

            entity.ToTable("higrometria");

            entity.HasIndex(e => e.DadosGeralId, "higrometria_dadosgeralid_fk");

            entity.HasIndex(e => e.PropostaId, "higrometria_propostaid_fk");

            entity.Property(e => e.HigrometriaId).HasColumnName("HigrometriaID");
            entity.Property(e => e.DadosGeralId).HasColumnName("DadosGeralID");
            entity.Property(e => e.DataLogger).HasMaxLength(255);
            entity.Property(e => e.HumidadeExterior).HasPrecision(10, 2);
            entity.Property(e => e.HumidadeInterior).HasPrecision(10, 2);
            entity.Property(e => e.NivelCo2)
                .HasPrecision(10, 2)
                .HasColumnName("NivelCO2");
            entity.Property(e => e.NivelHcho)
                .HasPrecision(10, 2)
                .HasColumnName("NivelHCHO");
            entity.Property(e => e.NivelTcov)
                .HasPrecision(10, 2)
                .HasColumnName("NivelTCOV");
            entity.Property(e => e.PontoOrvalho).HasPrecision(10, 2);
            entity.Property(e => e.PropostaId).HasColumnName("PropostaID");
            entity.Property(e => e.TempExterior).HasPrecision(10, 2);
            entity.Property(e => e.TempInterior).HasPrecision(10, 2);
            entity.Property(e => e.TempParedesInt).HasPrecision(10, 2);
            entity.Property(e => e.TempPontosFrios).HasPrecision(10, 2);

            entity.HasOne(d => d.DadosGeral).WithMany(p => p.Higrometria)
                .HasForeignKey(d => d.DadosGeralId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("higrometria_dadosgeralid_fk");

            entity.HasOne(d => d.Proposta).WithMany(p => p.Higrometria)
                .HasForeignKey(d => d.PropostaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("higrometria_propostaid_fk");
        });

        modelBuilder.Entity<Localidade>(entity =>
        {
            entity.HasKey(e => e.LocalidadeId).HasName("PRIMARY");

            entity.ToTable("localidade");

            entity.HasIndex(e => e.PaisId, "localidade_paisid_fk");

            entity.Property(e => e.LocalidadeId).HasColumnName("LocalidadeID");
            entity.Property(e => e.Regiao).HasMaxLength(255);
            entity.Property(e => e.PaisId).HasColumnName("PaisID");
            entity.Property(e => e.Regiao).HasMaxLength(255);

            entity.HasOne(d => d.Pais).WithMany(p => p.Localidades)
                .HasForeignKey(d => d.PaisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("localidade_paisid_fk");
        });

        modelBuilder.Entity<Pai>(entity =>
        {
            entity.HasKey(e => e.PaisId).HasName("PRIMARY");

            entity.ToTable("pais");

            entity.Property(e => e.PaisId).HasColumnName("PaisID");
            entity.Property(e => e.NomePais).HasMaxLength(255);
        });

        modelBuilder.Entity<Propostum>(entity =>
        {
            entity.HasKey(e => e.PropostaId).HasName("PRIMARY");

            entity.ToTable("proposta");

            entity.HasIndex(e => e.ClienteId, "proposta_clienteid_fk");

            entity.HasIndex(e => e.UtilizadorId, "proposta_utilizadorid_fk");

            entity.Property(e => e.PropostaId).HasColumnName("PropostaID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.CodigoProposta)
                .HasMaxLength(255)
                .HasComment("Codigo composto por AAAAMMMDD0000PropostaID");
            entity.Property(e => e.DataAceitacao).HasColumnType("datetime");
            entity.Property(e => e.DataProposta).HasColumnType("datetime");
            entity.Property(e => e.Estado).HasMaxLength(255);
            entity.Property(e => e.UtilizadorId).HasColumnName("UtilizadorID");
            entity.Property(e => e.ValorObra).HasPrecision(10, 2);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Proposta)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proposta_clienteid_fk");

            entity.HasOne(d => d.Utilizador).WithMany(p => p.Proposta)
                .HasForeignKey(d => d.UtilizadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proposta_utilizadorid_fk");
        });

        modelBuilder.Entity<Qualidadear>(entity =>
        {
            entity.HasKey(e => e.QualidadeArId).HasName("PRIMARY");

            entity.ToTable("qualidadear");

            entity.HasIndex(e => e.DadosGeralId, "qualidadear_dadosgeralid_fk");

            entity.HasIndex(e => e.VolumeId, "qualidadear_volumeid_fk");

            entity.Property(e => e.QualidadeArId).HasColumnName("QualidadeArID");
            entity.Property(e => e.DadosGeralId).HasColumnName("DadosGeralID");
            entity.Property(e => e.ObjetivoId).HasColumnName("ObjetivoID");
            entity.Property(e => e.VolumeId).HasColumnName("VolumeID");

            entity.HasOne(d => d.DadosGeral).WithMany(p => p.Qualidadears)
                .HasForeignKey(d => d.DadosGeralId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("qualidadear_dadosgeralid_fk");

            entity.HasOne(d => d.Volume).WithMany(p => p.Qualidadears)
                .HasForeignKey(d => d.VolumeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("qualidadear_volumeid_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Descricao).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(255);
        });

        modelBuilder.Entity<Sintoma>(entity =>
        {
            entity.HasKey(e => e.SintomaId).HasName("PRIMARY");

            entity.ToTable("sintoma");

            entity.HasIndex(e => e.DadosGeralId, "sintoma_dadosgeralid_fk");

            entity.HasIndex(e => e.PropostaId, "sintoma_propostaid_fk");

            entity.Property(e => e.SintomaId).HasColumnName("SintomaID");
            entity.Property(e => e.DadosGeralId).HasColumnName("DadosGeralID");
            entity.Property(e => e.PropostaId).HasColumnName("PropostaID");

            entity.HasOne(d => d.DadosGeral).WithMany(p => p.Sintomas)
                .HasForeignKey(d => d.DadosGeralId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sintoma_dadosgeralid_fk");

            entity.HasOne(d => d.Proposta).WithMany(p => p.Sintomas)
                .HasForeignKey(d => d.PropostaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sintoma_propostaid_fk");
        });

        modelBuilder.Entity<Tipojanela>(entity =>
        {
            entity.HasKey(e => e.TipoJanelaId).HasName("PRIMARY");

            entity.ToTable("tipojanela");

            entity.Property(e => e.TipoJanelaId).HasColumnName("TipoJanelaID");
            entity.Property(e => e.Descricao).HasMaxLength(255);
            entity.Property(e => e.TipoJanela1)
                .HasMaxLength(255)
                .HasColumnName("TipoJanela");
        });

        modelBuilder.Entity<Tipoobra>(entity =>
        {
            entity.HasKey(e => e.TipoObraId).HasName("PRIMARY");

            entity.ToTable("tipoobra");

            entity.Property(e => e.TipoObraId).HasColumnName("TipoObraID");
            entity.Property(e => e.Descricao).HasMaxLength(255);
            entity.Property(e => e.Observacoes).HasMaxLength(255);
        });

        modelBuilder.Entity<Tipotratamento>(entity =>
        {
            entity.HasKey(e => e.TipoTratamentoId).HasName("PRIMARY");

            entity.ToTable("tipotratamento");

            entity.Property(e => e.TipoTratamentoId).HasColumnName("TipoTratamentoID");
            entity.Property(e => e.TipoTratamentoNome).HasMaxLength(255);
        });

        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasKey(e => e.UtilizadorId).HasName("PRIMARY");

            entity.ToTable("utilizador");

            entity.HasIndex(e => e.RoleId, "utilizador_roleid_fk");

            entity.Property(e => e.UtilizadorId).HasColumnName("UtilizadorID");
            entity.Property(e => e.Login).HasMaxLength(255);
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.Pwp)
                .HasMaxLength(255)
                .HasColumnName("PWP");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Utilizadors)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("utilizador_roleid_fk");
        });

        modelBuilder.Entity<Volume>(entity =>
        {
            entity.HasKey(e => e.VolumeId).HasName("PRIMARY");

            entity.ToTable("volume");

            entity.Property(e => e.VolumeId).HasColumnName("VolumeID");
            entity.Property(e => e.SuperficieTotal).HasPrecision(10, 2);
            entity.Property(e => e.VolumeTotal).HasPrecision(10, 2);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
