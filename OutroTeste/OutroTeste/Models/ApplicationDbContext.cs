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
        public DbSet<ServicoUnidadeAtendimento> ServicosUnidadeAtendimento { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define a chave primária da classe ServicoUnidadeAtendimento
            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasKey(sua => new { sua.idServicoUnidadeAtendimento });

            // Define a relação de um para muitos entre ServicoUnidadeAtendimento e Servico
            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasOne(sua => sua.Servico)
                .WithMany(s => s.ServicosUnidadeAtendimento)
                .HasForeignKey(sua => sua.idServico);  

            // Define a relação de um para muitos entre ServicoUnidadeAtendimento e UnidadeAtendimento
            modelBuilder.Entity<ServicoUnidadeAtendimento>()
                .HasOne(sua => sua.UnidadeAtendimento)
                .WithMany(ua => ua.ServicosUnidadeAtendimento)
                .HasForeignKey(sua => sua.idUnidadeAtendimento);

            // Define a chave primária da classe Agenda
            modelBuilder.Entity<Agenda>().HasKey(a => a.idAgenda);

            // Define a relação de um para muitos entre ServicoUnidadeAtendimento e Agenda
            modelBuilder.Entity<Agenda>()
                .HasOne(a => a.ServicoUnidadeAtendimento)
                .WithMany(sua => sua.Agendas)
                .HasForeignKey(a => a.idServicoUnidadeAtendimento);

            base.OnModelCreating(modelBuilder);
        }
    }
}