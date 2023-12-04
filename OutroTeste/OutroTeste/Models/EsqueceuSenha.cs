using System.ComponentModel.DataAnnotations;

namespace agenda.Models
{
    public class EsqueceuSenha
    {
        [Required(ErrorMessage = "Por favor, preencha o CPF")]
        [Display(Name = "Digite o CPF registrado", Prompt = "Insira o CPF registrado")]
        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
        public string CPF { get; set; }

    }
}
