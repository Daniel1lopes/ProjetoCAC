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
        public int idAgendamento { get; set; }
        public DateTime dtAgendamento { get; set; }
        public int idAgenda { get; set; }
        public int idPessoa { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }
    }
}
