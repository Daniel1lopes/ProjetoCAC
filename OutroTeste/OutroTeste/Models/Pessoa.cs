using OutroTeste.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("Pessoa", Schema = "CACTB")] 
    public class Pessoa
    {
        [Key]
        [Column("idAgenda", TypeName = "int")]
        public int idAgenda { get; set; }
        [Column("dtAgenda", TypeName = "date")]
        public DateTime dtAgenda { get; set; }
        [Column("hrFim", TypeName = "time")]
        public TimeSpan? hrFim { get; set; }
        [Column("hrInicio", TypeName = "time")]
        public TimeSpan hrInicio { get; set; }
        [Column("nuReserva", TypeName = "smallint")]
        public short? nuReserva { get; set; }
        [Column("nuVagas", TypeName = "smallint")]
        public short nuVagas { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }

        // Chave estrangeira para ServicoUnidadeAtendimento
        [ForeignKey("Sexo")]
        [Column("idServicoUnidadeAtendimento", TypeName = "tinyint")]
        public short idSexo { get; set; }

        // Propriedade de navegação para ServicoUnidadeAtendimento
        // public virtual Sexo Sexos { get; set; }

    }
}
