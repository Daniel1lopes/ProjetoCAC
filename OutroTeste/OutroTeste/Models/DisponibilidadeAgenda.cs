namespace OutroTeste.Models
{
    public class DisponibilidadeAgenda
    {
        public int idAgenda { get; set; }
        public DateTime dtAgenda { get; set; }
        public TimeSpan hrFim { get; set; }
        public TimeSpan hrInicio { get; set; }
        public short nuReserva { get; set; }
        public short nuVagas { get; set; } 
        public bool icAtivo { get; set; }
        public short idServicoUnidadeAtendimento { get; set; } 
        public int nuQtdeAgendamento { get; set; }
        public int nuQtdeDisponivel { get; set; }
    }
}
