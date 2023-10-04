using Microsoft.EntityFrameworkCore;
using OutroTeste.Models;

namespace OutroTeste.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CentroAtendimento> CentroAtendimentos { get; set; }
        public DbSet<Especialidade> Especialidades { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<UnidadeAtendimento> UnidadesAtendimento { get; set; }
        public DbSet<ServicoUnidadeAtendimento> ServicoUnidadeAtendimento { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServicoUnidadeAtendimento>()
            .HasKey(sua => new { sua.idServico, sua.idUnidadeAtendimento });
            modelBuilder.Entity<ServicoUnidadeAtendimento>()
            .HasOne(sua => sua.Servico)
            .WithMany(s => s.UnidadesAtendimento)
            .HasForeignKey(sua => sua.idServico);
            modelBuilder.Entity<ServicoUnidadeAtendimento>()
            .HasOne(sua => sua.UnidadeAtendimento)
            .WithMany(u => u.Servicos)
            .HasForeignKey(sua => sua.idUnidadeAtendimento);
            base.OnModelCreating(modelBuilder);
        }
    }
}