using System.ComponentModel.DataAnnotations.Schema;
using OutroTeste.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OutroTeste.Models
{
    [Table("Agendamento", Schema = "CACTB")]
    public class Agendamento
    {
        [Key]
        public short idAgendamento { get; set; }
        public DateTime dtAgendamento { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }
        [ForeignKey("IdAgenda")]
        public short idAgenda { get; set; }
        [ForeignKey("IdPessoa")]
        public short idPessoa { get; set; }
    }
}
