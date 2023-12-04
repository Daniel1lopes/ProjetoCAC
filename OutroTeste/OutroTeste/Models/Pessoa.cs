using agenda.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace agenda.Models
{
    [Table("Pessoa", Schema = "CACTB")] 
    public class Pessoa 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Pessoa()
        {
            Agendamento = new HashSet<Agendamento>();
            Colaborador = new HashSet<Colaborador>();
            Dependente = new HashSet<Dependente>();
            Dependente1 = new HashSet<Dependente>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idPessoa { get; set; }

        [Required(ErrorMessage = "Por favor, preencha a data de nascimento")]
        [DataType(DataType.Date, ErrorMessage = "Formato de data inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        public DateTime dtNascimento { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o seu nome")]
        [StringLength(100)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "O nome não pode conter números.")]
        [Display(Name = "Nome da Pessoa", Prompt = "Nome")]
        public string nmPessoa { get; set; }


        [Required(ErrorMessage = "Por favor, preencha o número de telefone")] 
        [StringLength(12)]
        [Phone(ErrorMessage="O celular informado não é válido")]
        [RegularExpression(@"^\d{11,12}$", ErrorMessage = "O telefone deve conter de 11 a 12 dígitos.")]
        [Display(Name = "Número de Telefone", Prompt = "Número de Telefone")]
        public string nuTelefone { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o campo CPF")]
        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
        [Display(Name = "Número de CPF", Prompt = "Número de CPF")]
        public string nuCPF { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o campo senha")]
        [StringLength(50)]
        [Display(Name = "Senha", Prompt = "Senha")]
        public string coSenha { get; set; }

        [Required(ErrorMessage = "Por favor, confirme sua senha")]
        [Display(Name = "Confirmar senha", Prompt = "Confirmar Senha")]
        [NotMapped]
        public string coSenhaConfirmar { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o email")]
        [StringLength(70)]
        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de email válido")]
        [Display(Name = "Email", Prompt = "Email")]
        public string edEmail { get; set; }

        public bool icAtivo { get; set; }

        [ForeignKey("Sexo")]
        [Required(ErrorMessage = "Por favor, escolha um gênero.")]
        [Range(1, 2, ErrorMessage = "Campo gênero pendente")]
        public byte idSexo { get; set; }
        [ValidateNever]
        public virtual Sexo Sexo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Agendamento> Agendamento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Colaborador> Colaborador { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dependente> Dependente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dependente> Dependente1 { get; set; }

        public bool SenhaValida(string coSenha)
        {
            return coSenha == coSenha.GerarHash();
        }

        public void SetSenhaHash()
        {
            coSenha = coSenha.GerarHash();
        }

        public string SetNovaSenha(string novaSenha)
        {
            coSenha = novaSenha.GerarHash();
            return coSenha;
        }

        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            coSenha = novaSenha.GerarHash();
            return novaSenha;
        }

        public static bool ValidarCPF(string cpf)
        {
            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            foreach (var n in cpf)
            {
                if (!char.IsDigit(n))
                    return false;
            }

            if (cpf.All(c => c == cpf[0]))
                return false;

            var multiplicadores1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicadores2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var soma = tempCpf.Select((c, i) => (c - '0') * multiplicadores1[i]).Sum();

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCpf += digito;
            soma = tempCpf.Select((c, i) => (c - '0') * multiplicadores2[i]).Sum();

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}
