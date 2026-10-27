using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class BankaiContext : DbContext
{
    public BankaiContext()
    {
    }

    public BankaiContext(DbContextOptions<BankaiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agendamento> Agendamentos { get; set; }

    public virtual DbSet<Agendamentoservico> Agendamentoservicos { get; set; }

    public virtual DbSet<Chamadogarantium> Chamadogarantia { get; set; }

    public virtual DbSet<Disponibilidade> Disponibilidades { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<Pagamento> Pagamentos { get; set; }

    public virtual DbSet<Pessoa> Pessoas { get; set; }

    public virtual DbSet<Profissionalespecialidade> Profissionalespecialidades { get; set; }

    public virtual DbSet<Profissionalservico> Profissionalservicos { get; set; }

    public virtual DbSet<Servico> Servicos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("agendamento");

            entity.HasIndex(e => e.IdPagamento, "fk_Agendamento_Pagamento_idx");

            entity.HasIndex(e => e.IdProfissional, "fk_Agendamento_Pessoa1_idx");

            entity.HasIndex(e => e.IdCliente, "fk_Agendamento_Pessoa2_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comentario)
                .HasMaxLength(255)
                .HasColumnName("comentario");
            entity.Property(e => e.DescricaoServico)
                .HasMaxLength(255)
                .HasColumnName("descricaoServico");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.IdPagamento).HasColumnName("idPagamento");
            entity.Property(e => e.IdProfissional).HasColumnName("idProfissional");
            entity.Property(e => e.Nota)
                .HasColumnType("enum('MR','R','B','MB','E')")
                .HasColumnName("nota");
            entity.Property(e => e.StatusAgendamento)
                .HasColumnType("enum('Execucao','Concluido','Cancelado')")
                .HasColumnName("statusAgendamento");
            entity.Property(e => e.TipoCobranca)
                .HasColumnType("enum('F','O')")
                .HasColumnName("tipoCobranca");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.AgendamentoIdClienteNavigations)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Agendamento_Pessoa2");

            entity.HasOne(d => d.IdPagamentoNavigation).WithMany(p => p.Agendamentos)
                .HasForeignKey(d => d.IdPagamento)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Agendamento_Pagamento");

            entity.HasOne(d => d.IdProfissionalNavigation).WithMany(p => p.AgendamentoIdProfissionalNavigations)
                .HasForeignKey(d => d.IdProfissional)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Agendamento_Pessoa1");
        });

        modelBuilder.Entity<Agendamentoservico>(entity =>
        {
            entity.HasKey(e => new { e.IdAgendamento, e.IdServico }).HasName("PRIMARY");

            entity.ToTable("agendamentoservico");

            entity.HasIndex(e => e.IdAgendamento, "fk_AgendamentoServiço_Agendamento1_idx");

            entity.HasIndex(e => e.IdServico, "fk_AgendamentoServiço_Serviço1_idx");

            entity.Property(e => e.IdAgendamento).HasColumnName("idAgendamento");
            entity.Property(e => e.IdServico).HasColumnName("idServico");
            entity.Property(e => e.StatusServico)
                .HasColumnType("enum('E','C','CA')")
                .HasColumnName("statusServico");
            entity.Property(e => e.ValorProposto)
                .HasPrecision(10)
                .HasColumnName("valorProposto");

            entity.HasOne(d => d.IdAgendamentoNavigation).WithMany(p => p.Agendamentoservicos)
                .HasForeignKey(d => d.IdAgendamento)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_AgendamentoServiço_Agendamento1");

            entity.HasOne(d => d.IdServicoNavigation).WithMany(p => p.Agendamentoservicos)
                .HasForeignKey(d => d.IdServico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_AgendamentoServiço_Serviço1");
        });

        modelBuilder.Entity<Chamadogarantium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chamadogarantia");

            entity.HasIndex(e => e.IdAgendamento, "fk_ChamadoGarantia_Agendamento1_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataChamado)
                .HasColumnType("date")
                .HasColumnName("dataChamado");
            entity.Property(e => e.IdAgendamento).HasColumnName("idAgendamento");
            entity.Property(e => e.StatusGarantia)
                .HasColumnType("enum('S','A','R','C','E')")
                .HasColumnName("statusGarantia");

            entity.HasOne(d => d.IdAgendamentoNavigation).WithMany(p => p.Chamadogarantia)
                .HasForeignKey(d => d.IdAgendamento)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ChamadoGarantia_Agendamento1");
        });

        modelBuilder.Entity<Disponibilidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("disponibilidade");

            entity.HasIndex(e => e.IdPessoa, "fk_Disponibilidade_Pessoa1_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiaSemana)
                .HasMaxLength(13)
                .IsFixedLength()
                .HasColumnName("diaSemana");
            entity.Property(e => e.HorarioFim)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("horarioFim");
            entity.Property(e => e.HorarioInicio)
                .HasMaxLength(5)
                .IsFixedLength()
                .HasColumnName("horarioInicio");
            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.StatusAgenda)
                .HasColumnType("enum('D','ND')")
                .HasColumnName("statusAgenda");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Disponibilidades)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Disponibilidade_Pessoa1");
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("especialidade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pagamento");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FormaPagamento)
                .HasColumnType("enum('P','CC')")
                .HasColumnName("formaPagamento");
            entity.Property(e => e.StatusPagamento)
                .HasColumnType("enum('AP','P','R','A','C')")
                .HasColumnName("statusPagamento");
            entity.Property(e => e.ValorTransacao)
                .HasPrecision(10)
                .HasColumnName("valorTransacao");
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pessoa");

            entity.HasIndex(e => e.CpfCnpj, "cpf_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Telefone, "telefone_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CpfCnpj)
                .HasMaxLength(14)
                .IsFixedLength()
                .HasColumnName("cpfCnpj");
            entity.Property(e => e.EhProfissional).HasColumnName("ehProfissional");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Endereco)
                .HasMaxLength(100)
                .HasColumnName("endereco");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Senha)
                .HasMaxLength(30)
                .HasColumnName("senha");
            entity.Property(e => e.Telefone)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Profissionalespecialidade>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("profissionalespecialidade");

            entity.HasIndex(e => e.IdEspecialidade, "fk_ProfissionalEspecialidade_Especialidade1_idx");

            entity.HasIndex(e => e.IdPessoa, "fk_ProfissionalEspecialidade_Pessoa1_idx");

            entity.Property(e => e.IdEspecialidade).HasColumnName("idEspecialidade");
            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");

            entity.HasOne(d => d.IdEspecialidadeNavigation).WithMany()
                .HasForeignKey(d => d.IdEspecialidade)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ProfissionalEspecialidade_Especialidade1");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany()
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ProfissionalEspecialidade_Pessoa1");
        });

        modelBuilder.Entity<Profissionalservico>(entity =>
        {
            entity.HasKey(e => new { e.IdPessoa, e.IdServico }).HasName("PRIMARY");

            entity.ToTable("profissionalservico");

            entity.HasIndex(e => e.IdPessoa, "fk_ProfissionalServiço_Pessoa1_idx");

            entity.HasIndex(e => e.IdServico, "fk_ProfissionalServiço_Servico1_idx");

            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.IdServico).HasColumnName("idServico");
            entity.Property(e => e.Preco)
                .HasPrecision(10)
                .HasColumnName("preco");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Profissionalservicos)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ProfissionalServiço_Pessoa1");

            entity.HasOne(d => d.IdServicoNavigation).WithMany(p => p.Profissionalservicos)
                .HasForeignKey(d => d.IdServico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ProfissionalServiço_Servico1");
        });

        modelBuilder.Entity<Servico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("servico");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Nome, "nome_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
