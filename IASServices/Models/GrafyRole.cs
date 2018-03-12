using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    public partial class GrafyRole
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("id_grafu")]
        public long IdGrafu { get; set; }
        [Column("user", TypeName = "varchar(50)")]
        public string User { get; set; }
        [Column("role", TypeName = "varchar(50)")]
        public string Role { get; set; }
        [Column("nazwa", TypeName = "varchar(50)")]
        public string Nazwa { get; set; }
        [Column("id_parent")]
        public long? IdParent { get; set; }
        [Column("typ", TypeName = "varchar(10)")]
        public string Typ { get; set; }

        [ForeignKey("IdGrafu")]
        [InverseProperty("GrafyRole")]
        public virtual GrafyGraf IdGrafuNavigation { get; set; }
    }
}
