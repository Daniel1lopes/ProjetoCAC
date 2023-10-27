using System.ComponentModel.DataAnnotations.Schema;
using agenda.Models;
using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    [Table("CACTB.Dependente", Schema = "CACTB")]
    public class Dependente
    {
        [Key]
        public int idDependente { get; set; }

        public bool icAtivo { get; set; }

        public int idPessoaDependente { get; set; }

        public int idPessoaResponsavel { get; set; }

        public byte idTipoDependente { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public virtual TipoDependente TipoDependente { get; set; }
    }
}
