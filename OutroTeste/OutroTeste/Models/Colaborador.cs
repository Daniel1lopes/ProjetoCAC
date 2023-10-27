using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("Colaborador", Schema = "CACTB")]
    public class Colaborador
    {
        [Key]
        public int idColaborador { get; set; }

        public bool icAdministrador { get; set; }

        public bool icAtivo { get; set; }

        public int idPessoa { get; set; }

        public virtual ICollection<CentroAtendimentoColaborador> CentroAtendimentoColaborador { get; set; }

        public virtual Pessoa Pessoa { get; set; }
    }
}
