using Microsoft.EntityFrameworkCore.Metadata.Internal;
using agenda.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace agenda.Models
{
    [Table("ServicoUnidadeAtendimento", Schema = "CACTB")] // Especifique o nome da tabela
    public class ServicoUnidadeAtendimento
    {
        public ServicoUnidadeAtendimento()
        {
            Agenda = new HashSet<Agenda>();
            HorarioServico = new HashSet<HorarioServico>();
        }

        [Key]
        public short idServicoUnidadeAtendimento { get; set; }

        [ForeignKey("UnidadeAtendimento")]
        [Column("idUnidadeAtendimento", TypeName = "smallint")]
        public short idUnidadeAtendimento { get; set; }
        [ForeignKey("Servico")]
        [Column(TypeName = "smallint")]
        public short idServico { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }

        public virtual UnidadeAtendimento UnidadeAtendimento { get; set; }
        public virtual Servico Servico { get; set; }

        public virtual ICollection<Agenda> Agenda { get; set; }
        public virtual ICollection<HorarioServico> HorarioServico { get; set; }

    }
}
