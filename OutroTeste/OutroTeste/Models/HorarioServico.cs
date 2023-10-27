using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace agenda.Models
{
    [Table("HorarioServico", Schema = "CACTB")]

    public class HorarioServico
    {
        [Key]
        public int idHorarioServico { get; set; }

        public TimeSpan? hrFim { get; set; }

        public TimeSpan hrInicio { get; set; }

        public short? nuReserva { get; set; }

        public short nuVagas { get; set; }

        public bool icAtivo { get; set; }

        public byte idDiaSemana { get; set; }

        public short idServicoUnidadeAtendimento { get; set; }

        public virtual DiaSemana DiaSemana { get; set; }

        public virtual ServicoUnidadeAtendimento ServicoUnidadeAtendimento { get; set; }
    }
}
