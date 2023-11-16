using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Por favor, insira o nome ou email.")]
        [Display(Name = "Nome ou Email do usuário", Prompt = "Nome ou Email do usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Por favor, insira a senha.")]
        [Display(Name = "Senha do usuário", Prompt = "Senha do usuário")]
        public string Senha { get; set; }
    }
}
