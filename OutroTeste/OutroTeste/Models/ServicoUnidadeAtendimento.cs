using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OutroTeste.Models;


namespace OutroTeste.Models
{
    [Table("ServicoUnidadeAtendimento", Schema = "CACTB")]
    public class ServicoUnidadeAtendimento
    {
        [Key]
        [Column("idServico")] // Defina o nome correto da coluna
        public short idServico { get; set; }
        [Key]
        [Column("idUnidadeAtendimento")] // Defina o nome correto da coluna
        public short idUnidadeAtendimento { get; set; }
        [ForeignKey("idServico")]
        public Servico Servico { get; set; }
        public UnidadeAtendimento UnidadeAtendimento { get; set; }
    }
}
