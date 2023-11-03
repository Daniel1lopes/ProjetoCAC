using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    public class EsqueceuSenha
    {
        [Required]
        [EmailAddress(ErrorMessage = "Por favor, insira um email válido")]
        [Display(Name = "Digite o email registrado", Prompt = "Insira o email registrado")]
        [StringLength(70)]
        public string Email { get; set; }
        public bool EmailMandado { get; set; }
    }
}
