using OutroTeste.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("Agenda", Schema = "CACTB")] // Especifique o nome da tabela
    public class Agenda
    {
        [Key]
        public int idAgenda { get; set; }

        public DateTime dtAgenda { get; set; }

        public TimeSpan? hrFim { get; set; }

        public TimeSpan hrInicio { get; set; }

        public short? nuReserva { get; set; }

        public short nuVagas { get; set; }

        public bool icAtivo { get; set; }

        // Chave estrangeira para ServicoUnidadeAtendimento
        [ForeignKey("ServicoUnidadeAtendimento")]
        public short idServicoUnidadeAtendimento { get; set; }

        // Propriedade de navegação para ServicoUnidadeAtendimento
        public virtual ServicoUnidadeAtendimento ServicoUnidadeAtendimento { get; set; }
    }
}
