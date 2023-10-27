using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("CentroAtendimentoColaborador", Schema = "CACTB")]
    public class CentroAtendimentoColaborador
    {
        [Key]
        public int idCentroAtendimentoColaborador { get; set; }

        public short idCentroAtendimento { get; set; }

        public int idColaborador { get; set; }

        public bool icAtivo { get; set; }

        public virtual CentroAtendimento CentroAtendimento { get; set; }

        public virtual Colaborador Colaborador { get; set; }
    }
}
