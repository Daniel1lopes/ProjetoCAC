using agenda.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("Agenda", Schema = "CACTB")] 
    public class Agenda
    {
        public Agenda()
        {
            Agendamento = new HashSet<Agendamento>();
        }

        [Key]
        [Column("idAgenda", TypeName = "int")]
        public int idAgenda { get; set; }
        [Column("dtAgenda", TypeName = "date")]
        public DateTime dtAgenda { get; set; }
        [Column("hrFim", TypeName = "time")]
        public TimeSpan? hrFim { get; set; }
        [Column("hrInicio", TypeName = "time")]
        public TimeSpan hrInicio { get; set; }
        [Column("nuReserva", TypeName = "smallint")]
        public short? nuReserva { get; set; }
        [Column("nuVagas", TypeName = "smallint")]
        public short nuVagas { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }

        [ForeignKey("ServicoUnidadeAtendimento")]
        [Column("idServicoUnidadeAtendimento", TypeName = "smallint")]
        public short idServicoUnidadeAtendimento { get; set; }

        public virtual ServicoUnidadeAtendimento ServicoUnidadeAtendimento { get; set; }

        public ICollection<Agendamento> Agendamento { get; set; }
    }
}