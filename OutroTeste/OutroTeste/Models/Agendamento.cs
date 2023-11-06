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

        [Column(TypeName = "smalldatetime")]
        public DateTime dtAgendamento { get; set; }

        public bool icAtivo { get; set; }

        public int idAgenda { get; set; }

        public int idPessoa { get; set; }

        [ForeignKey("idAgenda")]
        public virtual Agenda Agenda { get; set; }

        [ForeignKey("idPessoa")]
        public virtual Pessoa Pessoa { get; set; }
    }
}
