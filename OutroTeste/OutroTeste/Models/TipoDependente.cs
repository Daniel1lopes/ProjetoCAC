using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("TipoDependente", Schema = "CACTB")]
    public class TipoDependente
    {
        public TipoDependente()
        {
            Dependente = new HashSet<Dependente>();
        }

        [Key]
        public byte idTipoDependente { get; set; }

        [Required]
        [StringLength(20)]
        public string nmTipoDependente { get; set; }

        public virtual ICollection<Dependente> Dependente { get; set; }
    }
}
