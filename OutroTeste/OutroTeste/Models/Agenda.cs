using OutroTeste.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("Agenda", Schema = "CACTB")]

    public class Agenda
    {
        [Key]
        public int idAgenda { get; set; }
        public DateTime dtAgenda { get; set; }
        public TimeSpan hrInicio { get; set; }
        public TimeSpan hrFim { get; set; }
        public int nuVagas { get; set; }
        public int nuReserva { get; set; }
        public int nuAgendamentos { get; set; }
        public short idServico { get; set; } 
        public short idUnidadeAtendimento { get; set; } 

        [ForeignKey("idServico, idUnidadeAtendimento")] 
        public ServicoUnidadeAtendimento servicoUnidadeAtendimento { get; set; }

    }
}