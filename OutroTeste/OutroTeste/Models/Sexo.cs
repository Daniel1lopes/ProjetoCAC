using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("Sexo", Schema = "CACTB")]
    public class Sexo
    {
        [Key]
        public byte idSexo { get; set; }

        [Required]
        [StringLength(9)]
        public string nmSexo { get; set; }

        public virtual ICollection<Pessoa> Pessoa { get; set; }
    }
}
