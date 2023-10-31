using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    public class DiaSemana
    {
        public DiaSemana()
        {
            HorarioServico = new HashSet<HorarioServico>();
        }

        [Key]
        public byte idDiaSemana { get; set; }

        [Required]
        [StringLength(13)]
        public string deDiaSemanaExtenso { get; set; }

        [Required]
        [StringLength(3)]
        public string deDiaSemanaAbreviado { get; set; }

        public byte nuDiaSemana { get; set; }

        public virtual ICollection<HorarioServico> HorarioServico { get; set; }
    }
}
