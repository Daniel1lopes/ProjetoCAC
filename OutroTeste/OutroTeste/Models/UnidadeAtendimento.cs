using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("UnidadeAtendimento", Schema = "CACTB")]
    public class UnidadeAtendimento
    {
        [Key]
        [Column("idUnidadeAtendimento", TypeName = "smallint")]
        public short idUnidadeAtendimento { get; set; }
        [Column("nmUnidadeAtendimento", TypeName = "varchar(100)")]
        public string nmUnidadeAtendimento { get; set; }
        public ICollection<ServicoUnidadeAtendimento> Servicos { get; set; }
    }
}