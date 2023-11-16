using agenda.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    public class EditarPessoa
    {
        [Required(ErrorMessage = "Por favor, preencha a data de nascimento")]
        [Column(TypeName = "date")]
        public DateTime dtNascimento { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o seu nome")]
        [StringLength(100)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "O nome não pode conter números.")]
        public string nmPessoa { get; set; }

        [StringLength(50)]
        public string coSenha { get; set; }

        [StringLength(50)]
        public string coSenhaConfirmar { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o número de telefone")] 
        [StringLength(12)]
        [Phone(ErrorMessage="O celular informado não é válido")]
        [RegularExpression(@"^\d{11,12}$", ErrorMessage = "O telefone deve conter de 11 a 12 dígitos.")]
        public string nuTelefone { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o campo CPF")]
        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
        public string nuCPF { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o email")]
        [StringLength(70)]
        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de email válido")]
        public string edEmail { get; set; }

        [Required(ErrorMessage = "Por favor, escolha um gênero.")]
        [Range(1, 2, ErrorMessage = "Campo gênero pendente")]
        public byte idSexo { get; set; }
    }
}
