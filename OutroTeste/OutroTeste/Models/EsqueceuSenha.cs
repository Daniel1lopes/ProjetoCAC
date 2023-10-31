using System.ComponentModel.DataAnnotations;

namespace OutroTeste.Models
{
    public class EsqueceuSenha
    {
        [Required,EmailAddress, Display(Name ="Digite o email registrado"), StringLength(70)]
        public string Email { get; set; }
        public bool EmailMandado { get; set; }
    }
}
