using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Por favor, insira o nome do usuário.")]
        [Display(Name = "Nome do usuário", Prompt = "Nome do usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Por favor, insira a senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha do usuário", Prompt = "Senha do usuário")]
        public string Senha { get; set; }
    }
}
