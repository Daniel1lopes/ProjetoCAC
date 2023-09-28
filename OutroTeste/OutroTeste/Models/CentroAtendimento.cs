using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("CentroAtendimento", Schema = "CACTB")] // Especifique o nome da tabela
    public class CentroAtendimento
    {
        [Key]
        [Column("idCentroAtendimento", TypeName = "smallint")]
        public short idCentroAtendimento { get; set; }
        [Column("nmCentroAtendimento", TypeName = "varchar(100)")]
        public string nmCentroAtendimento { get; set; }
        public ICollection<Especialidade> Especialidades { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }
    }
}