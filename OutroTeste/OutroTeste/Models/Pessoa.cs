using OutroTeste.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutroTeste.Models
{
    [Table("Pessoa", Schema = "CACTB")] 
    public class Pessoa
    {
        public int idPessoa { get; set; }

        [Column(TypeName = "date")]
        public DateTime dtNascimento { get; set; }

        [Required]
        [StringLength(100)]
        public string nmPessoa { get; set; }

        [Required]
        [StringLength(12)]
        public string nuTelefone { get; set; }

        [Required]
        [StringLength(11)]
        public string nuCPF { get; set; }

        [StringLength(50)]
        public string coSenha { get; set; }

        [Required]
        [StringLength(70)]
        public string edEmail { get; set; }

        public bool icAtivo { get; set; }

        public byte idSexo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Agendamento> Agendamento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Colaborador> Colaborador { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dependente> Dependente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dependente> Dependente1 { get; set; }

        public virtual Sexo Sexo { get; set; }

    }
}
