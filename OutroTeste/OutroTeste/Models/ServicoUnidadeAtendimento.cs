using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OutroTeste.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OutroTeste.Models
{
    [Table("ServicoUnidadeAtendimento", Schema = "CACTB")] // Especifique o nome da tabela
    public class ServicoUnidadeAtendimento
    {
        [Key]
        public short idServicoUnidadeAtendimento { get; set; }

        [ForeignKey("UnidadeAtendimento")]
        [Column("idUnidadeAtendimento", TypeName = "smallint")]
        public short idUnidadeAtendimento { get; set; }
        public virtual UnidadeAtendimento UnidadeAtendimento { get; set; }

        [ForeignKey("Servico")]
        [Column(TypeName = "smallint")]
        public short idServico { get; set; }
        public virtual Servico Servico { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }

        public ICollection<Agenda> Agendas { get; set; }

    }
}
