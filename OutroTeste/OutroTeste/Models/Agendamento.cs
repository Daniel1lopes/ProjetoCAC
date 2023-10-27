using System.ComponentModel.DataAnnotations.Schema;
using agenda.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    [Table("Agendamento", Schema = "CACTB")]
    public class Agendamento
    {
        [Key]
        public int idAgendamento { get; set; }
        public DateTime dtAgendamento { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }
       // [ForeignKey("Agenda")]
        public int idAgenda { get; set; }
       //  public virtual Agenda Agendas { get; set; }
       // public ICollection<Agenda> Agenda { get; set; }

       // public virtual ServicoUnidadeAtendimento ServicosUnidadeAtendimento { get; set; }
       // [ForeignKey("Pessoa")]
       // public short idPessoa { get; set; }
       // public virtual Pessoa Pessoas { get; set; }
    }
}
