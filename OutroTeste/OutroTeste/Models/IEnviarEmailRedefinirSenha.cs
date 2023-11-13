using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace OutroTeste.Models
{
    public interface IEnviarEmailRedefinirSenha
    {
        public bool Enviar(string email, string assunto, string mensagem);
    }
}
