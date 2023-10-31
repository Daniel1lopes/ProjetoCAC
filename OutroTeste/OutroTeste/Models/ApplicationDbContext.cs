using Microsoft.EntityFrameworkCore;
using agenda.Models;

namespace agenda.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CentroAtendimento> CentroAtendimentos { get; set; }
        public DbSet<Especialidade> Especialidades { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<UnidadeAtendimento> UnidadesAtendimento { get; set; }
        public DbSet<ServicoUnidadeAtendimento> ServicosUnidadeAtendimento { get; set; }
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Dependente> Dependentes { get; set; }
        public DbSet<TipoDependente> TipoDependentes { get; set; }
        public DbSet<DisponibilidadeAgenda> DisponibilidadeAgendas { get; set; }
        public DbSet<HorarioServico> HorarioServicos { get; set; }
        public DbSet<DiaSemana> DiaSemanas { get; set; }
        public DbSet<Sexo> Sexos { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<CentroAtendimentoColaborador> CentroAtendimentoColaboradores { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasKey(sua => new { sua.idServicoUnidadeAtendimento });

            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasOne(sua => sua.Servico)
                .WithMany(s => s.ServicoUnidadeAtendimento)
                .HasForeignKey(sua => sua.idServico);

            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasOne(sua => sua.UnidadeAtendimento)
                .WithMany(ua => ua.ServicoUnidadeAtendimento)
                .HasForeignKey(sua => sua.idUnidadeAtendimento);

            modelBuilder.Entity<Agenda>().HasKey(a => a.idAgenda);

            modelBuilder.Entity<Agenda>()
                .HasOne(a => a.ServicoUnidadeAtendimento)
                .WithMany(sua => sua.Agenda)
                .HasForeignKey(a => a.idServicoUnidadeAtendimento);

            modelBuilder.Entity<DisponibilidadeAgenda>().HasNoKey().ToView("DisponibilidadeAgenda", "cacvw");

            modelBuilder.Entity<Agenda>()
            .HasMany(e => e.Agendamento)
            .WithOne(e => e.Agenda)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CentroAtendimento>()
                .Property(e => e.deCentroAtendimento)
                .IsUnicode(false);

            modelBuilder.Entity<CentroAtendimento>()
                .Property(e => e.nmCentroAtendimento)
                .IsUnicode(false);

            modelBuilder.Entity<CentroAtendimento>()
                .HasMany(e => e.CentroAtendimentoColaborador)
                .WithOne(e => e.CentroAtendimento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CentroAtendimento>()
                .HasMany(e => e.Especialidade)
                .WithOne(e => e.CentroAtendimento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CentroAtendimento>()
                .HasMany(e => e.UnidadeAtendimento)
                .WithOne(e => e.CentroAtendimento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Colaborador>()
                .HasMany(e => e.CentroAtendimentoColaborador)
                .WithOne(e => e.Colaborador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiaSemana>()
                .Property(e => e.deDiaSemanaExtenso)
                .IsUnicode(false);

            modelBuilder.Entity<DiaSemana>()
                .Property(e => e.deDiaSemanaAbreviado)
                .IsUnicode(false);

            modelBuilder.Entity<DiaSemana>()
                .HasMany(e => e.HorarioServico)
                .WithOne(e => e.DiaSemana)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Especialidade>()
                .Property(e => e.deEspecialidade)
                .IsUnicode(false);

            modelBuilder.Entity<Especialidade>()
                .Property(e => e.nmEspecialidade)
                .IsUnicode(false);

            modelBuilder.Entity<Especialidade>()
                .HasMany(e => e.Servico)
                .WithOne(e => e.Especialidade)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.nmPessoa)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.nuTelefone)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.nuCPF)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.coSenha)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.edEmail)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.Agendamento)
                .WithOne(e => e.Pessoa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.Colaborador)
                .WithOne(e => e.Pessoa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.Dependente)
                .WithOne(e => e.Pessoa)
                .HasForeignKey(e => e.idPessoaDependente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.Dependente1)
                .WithOne(e => e.Pessoa1)
                .HasForeignKey(e => e.idPessoaResponsavel)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Servico>()
                .Property(e => e.deServico)
                .IsUnicode(false);

            modelBuilder.Entity<Servico>()
                .Property(e => e.nmServico)
                .IsUnicode(false);

            modelBuilder.Entity<Servico>()
                .HasMany(e => e.ServicoUnidadeAtendimento)
                .WithOne(e => e.Servico)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasMany(e => e.Agenda)
                .WithOne(e => e.ServicoUnidadeAtendimento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasMany(e => e.HorarioServico)
                .WithOne(e => e.ServicoUnidadeAtendimento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sexo>()
                .Property(e => e.nmSexo)
                .IsUnicode(false);

            modelBuilder.Entity<Sexo>()
                .HasMany(e => e.Pessoa)
                .WithOne(e => e.Sexo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoDependente>()
                .Property(e => e.nmTipoDependente)
                .IsUnicode(false);

            modelBuilder.Entity<TipoDependente>()
                .HasMany(e => e.Dependente)
                .WithOne(e => e.TipoDependente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeAtendimento>()
                .Property(e => e.deUnidadeAtendimento)
                .IsUnicode(false);

            modelBuilder.Entity<UnidadeAtendimento>()
                .Property(e => e.nmUnidadeAtendimento)
                .IsUnicode(false);

            modelBuilder.Entity<UnidadeAtendimento>()
                .HasMany(e => e.ServicoUnidadeAtendimento)
                .WithOne(e => e.UnidadeAtendimento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sexo>().HasData(
                new Sexo { idSexo = 1, nmSexo = "Mulher" },
                new Sexo { idSexo = 2, nmSexo = "Homem" });

            base.OnModelCreating(modelBuilder);
        }
    }
}