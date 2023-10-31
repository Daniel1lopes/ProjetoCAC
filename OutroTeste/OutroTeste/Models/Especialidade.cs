﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace agenda.Models
{
    [Table("Especialidade", Schema = "CACTB")]
    public class Especialidade
    {
        public Especialidade()
        {
            Servico = new HashSet<Servico>();
        }
        [Key]
        [Column("idEspecialidade", TypeName = "smallint")]
        public short idEspecialidade { get; set; }
        [Column("deEspecialidade", TypeName = "varchar(500)")]
        public string? deEspecialidade { get; set; }
        [Column("imEspecialidade", TypeName = "varbinary(max)")]
        public byte[]? imEspecialidade { get; set; }
        [Column("nmEspecialidade", TypeName = "varchar(50)")]
        public string nmEspecialidade { get; set; }
        [Column("icAtivo", TypeName = "bit")]
        public bool icAtivo { get; set; }
        [ForeignKey("CentroAtendimento")]
        public short idCentroAtendimento { get; set; }
        public virtual CentroAtendimento CentroAtendimento { get; set; }
        public ICollection<Servico> Servico { get; set; }
    }
}
