using agenda.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required(ErrorMessage = "Digite a data de nascimento")]
        [Column(TypeName = "date")]
        public DateTime dtNascimento { get; set; }

        [Required(ErrorMessage = "Digite o nome do usuário")]
        [StringLength(100)]
        public string nmPessoa { get; set; }


        [Required(ErrorMessage = "Digite o número do telefone")]
        [StringLength(12)]
        public string nuTelefone { get; set; }

        [Required(ErrorMessage = "Digite o número do CPF")]
        [StringLength(11)]
        public string nuCPF { get; set; }

        [Required(ErrorMessage = "Digite a senha")]
        [StringLength(50)]
        public string coSenha { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        public string coSenhaConfirmar { get; set; }

        [Required(ErrorMessage = "Digite o email")]
        [StringLength(70)]
        public string edEmail { get; set; }

        public bool icAtivo { get; set; }

        [ForeignKey("Sexo")]
        [Required(ErrorMessage = "Por favor, escolha um gênero.")]
        [Range(1, 2, ErrorMessage = "O gênero deve ser (Mulher) ou (Homem).")]
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

        public void SetNovaSenha(string novaSenha)
        {
            coSenha = novaSenha.GerarHash();
        }

        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            coSenha = novaSenha.GerarHash();
            return novaSenha;
        }
    }
}
