using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    public class EsqueceuSenha
    {
        [Required(ErrorMessage = "Por favor, preencha o email")]
        [EmailAddress(ErrorMessage = "Por favor, insira um email válido")]
        [Display(Name = "Digite o email registrado", Prompt = "Insira o email registrado")]
        [StringLength(70)]
        public string Email { get; set; }

    }
}
