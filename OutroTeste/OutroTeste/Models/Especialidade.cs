using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("Especialidade", Schema = "CACTB")]
    public class Especialidade
    {
        [Key]
        [Column("idEspecialidade", TypeName = "smallint")]
        public short idEspecialidade { get; set; }
        [Column("deEspecialidade", TypeName = "varchar(500)")]
        public string? deEspecialidade { get; set; }
        [Column("imEspecialidade", TypeName = "varbinary(max)")]
        public byte[] imEspecialidade { get; set; }
        [Column("nmEspecialidade", TypeName = "varchar(50)")]
        public string nmEspecialidade { get; set; }
        [Column("idCentroAtendimento", TypeName = "smallint")]

        public short idCentroAtendimento { get; set; }
        [ForeignKey("idCentroAtendimento")]
        [InverseProperty("Especialidades")]
        public CentroAtendimento CentroAtendimento { get; set; }
        [InverseProperty("Especialidade")]
        public ICollection<Servico> Servicos { get; set; }
    }
}
