using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Por favor, insira o CPF.")]
        [Display(Name = "CPF do usuário", Prompt = "CPF do usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Por favor, insira a senha.")]
        [Display(Name = "Senha do usuário", Prompt = "Senha do usuário")]
        public string Senha { get; set; }
    }
}
