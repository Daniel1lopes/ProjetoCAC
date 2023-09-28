using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("Servico", Schema = "CACTB")]
    public class Servico
    {
        [Key]
        [Column("idServico", TypeName = "smallint")]
        public short idServico { get; set; }
        [Column("deServico", TypeName = "varchar(100)")]
        public string deServico { get; set; }
        [Column("idEspecialidade", TypeName = "smallint")]
        public short idEspecialidade { get; set; }
        [ForeignKey("idEspecialidade")]
        [InverseProperty("Servicos")]
        public Especialidade Especialidade { get; set; }
        public ICollection<ServicoUnidadeAtendimento> UnidadesAtendimento { get; set; }
    }
}
