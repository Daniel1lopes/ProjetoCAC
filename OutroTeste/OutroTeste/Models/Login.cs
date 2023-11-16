using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Por favor, insira o email.")]
        [Display(Name = "Email do usuário", Prompt = "Email do usuário")]
        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de email válido")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Por favor, insira a senha.")]
        [Display(Name = "Senha do usuário", Prompt = "Senha do usuário")]
        public string Senha { get; set; }
    }
}
