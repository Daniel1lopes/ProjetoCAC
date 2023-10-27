using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("Servico", Schema = "CACTB")]
    public class Servico
    {
        [Key]
        [Column("idServico", TypeName = "smallint")]
        public short idServico { get; set; }
        [Column("deServico", TypeName = "varchar(500)")]
        public string? deServico { get; set; }
        [Column("imServico", TypeName = "varbinary(max)")]
        public byte[]? imServico { get; set; }
        [Column("nmServico", TypeName = "varchar(100)")]
        public string nmServico { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }
        [ForeignKey("Especialidade")]
        [Column("idEspecialidade", TypeName = "smallint")]
        public short idEspecialidade { get; set; }
        public virtual Especialidade Especialidade { get; set; }
        public ICollection<ServicoUnidadeAtendimento> ServicosUnidadeAtendimento { get; set; }
    }
}
