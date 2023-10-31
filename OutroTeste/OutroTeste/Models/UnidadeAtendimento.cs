using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("UnidadeAtendimento", Schema = "CACTB")]
    public class UnidadeAtendimento
    {
        public UnidadeAtendimento()
        {
            ServicoUnidadeAtendimento = new HashSet<ServicoUnidadeAtendimento>();
        }
        [Key]
        [Column("idUnidadeAtendimento", TypeName = "smallint")]
        public short idUnidadeAtendimento { get; set; }
        [Column("deUnidadeAtendimento", TypeName = "varchar(200)")]
        public string deUnidadeAtendimento { get; set; }
        [Column("imUnidadeAtendimento", TypeName = "varbinary(max)")]
        public byte[]? imUnidadeAtendimento { get; set; }
        [Column("nmUnidadeAtendimento", TypeName = "varchar(100)")]
        public string nmUnidadeAtendimento { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }
        [ForeignKey("CentroAtendimento")]
        public short idCentroAtendimento { get; set; }
        public virtual CentroAtendimento CentroAtendimento { get; set; }
        public ICollection<ServicoUnidadeAtendimento> ServicoUnidadeAtendimento { get; set; }
    }
}