using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("Sexo", Schema = "CACTB")]
    public class Sexo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Sexo()
        {
            Pessoa = new HashSet<Pessoa>();
        }

        [Key]
        public byte idSexo { get; set; } 

        [Required]
        [StringLength(9)]
        public string nmSexo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pessoa> Pessoa { get; set; }
    }
}
