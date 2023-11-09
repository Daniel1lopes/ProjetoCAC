using agenda.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    public class AgendamentoFull
    {
        public int idAgendamento { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime dtAgendamento { get; set; }
		public bool icAtivoAgendamento { get; set; }
        public int idPessoa {  get; set; }
	    public int idAgenda { get; set; }

        public DateTime dtAgenda { get; set; }
        [Column("hrFim", TypeName = "time")]
        public TimeSpan? hrFim {  get; set; }
        [Column("hrInicio", TypeName = "time")]
        public TimeSpan hrInicio { get; set; }
        [Column("nuReserva", TypeName = "smallint")]
        public short? nuReserva { get; set; }
        [Column("nuVagas", TypeName = "smallint")]
        public short nuVagas { get; set; }
        [Column("icAtivoAgenda", TypeName = "bit")]
        public bool icAtivoAgenda {  get; set; }

        [Column("idServicoUnidadeAtendimento", TypeName = "smallint")]
        public short idServicoUnidadeAtendimento {  get; set; }
        [Column("idUnidadeAtendimento", TypeName = "smallint")]
        public short idUnidadeAtendimento {  get; set; }
        [Column("icAtivoServicoUnidadeAtendimento", TypeName = "bit")]
        public bool icAtivoServicoUnidadeAtendimento {  get; set; }


        [Column("idServico", TypeName = "smallint")]
        public short idServico { get; set; }
        [Column("deServico", TypeName = "varchar(500)")]
        public string? deServico { get; set; }
        [Column("imServico", TypeName = "varbinary(max)")]
        public byte[]? imServico { get; set; }
        [Column("nmServico", TypeName = "varchar(100)")]
        public string nmServico { get; set; }
        [Column("icAtivoServico", TypeName = "bit")]
        public bool icAtivoServico { get; set; }

        [Column("idEspecialidade", TypeName = "smallint")]
        public short idEspecialidade { get; set; }
        [Column("deEspecialidade", TypeName = "varchar(500)")]
        public string? deEspecialidade { get; set; }
        [Column("imEspecialidade", TypeName = "varbinary(max)")]
        public byte[]? imEspecialidade { get; set; }
        [Column("nmEspecialidade", TypeName = "varchar(50)")]
        public string nmEspecialidade { get; set; }
        [Column("icAtivoEspecialidade", TypeName = "bit")]
        public bool icAtivoEspecialidade { get; set; }

        [Column("idCentroAtendimento", TypeName = "smallint")]
        public short idCentroAtendimento { get; set; }
        [Column("deCentroAtendimento", TypeName = "varchar(500)")]
        public string? deCentroAtendimento { get; set; }
        [Column("imCentroAtendimento", TypeName = "varbinary(max)")]
        public byte[]? imCentroAtendimento { get; set; }
        [Column("nmCentroAtendimento", TypeName = "varchar(100)")]
        public string nmCentroAtendimento { get; set; }
        [Column("icAtivoCentroAtendimento", TypeName = "bit")]
        public bool icAtivoCentroAtendimento { get; set; }

        [Column("deUnidadeAtendimento", TypeName = "varchar(200)")]
        public string deUnidadeAtendimento { get; set; }
        [Column("nmUnidadeAtendimento", TypeName = "varchar(100)")]
        public string nmUnidadeAtendimento { get; set; }
    }
}
