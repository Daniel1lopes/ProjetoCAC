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
        public int idUnidadeAtendimento { get; set; }
        public virtual UnidadeAtendimento UnidadeAtendimento { get; set; }

        [Column(TypeName = "smallint")]
        [ForeignKey("Servico")]
        public int idServico { get; set; }
        public virtual Servico Servico { get; set; }

        public bool icAtivo { get; set; }

        public ICollection<Agenda> Agendas { get; set; }

    }
}
