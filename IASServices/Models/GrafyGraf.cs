using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    public partial class GrafyGraf
    {
        public GrafyGraf()
        {
            GrafyRole = new HashSet<GrafyRole>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("nazwa", TypeName = "varchar(50)")]
        public string Nazwa { get; set; }
        [Column("dataUtworzenia", TypeName = "date")]
        public DateTime? DataUtworzenia { get; set; }
        [Column("opis", TypeName = "varchar(max)")]
        public string Opis { get; set; }
        [Column("xml", TypeName = "text")]
        public string Xml { get; set; }
        [Column("opis2", TypeName = "varchar(max)")]
        public string Opis2 { get; set; }

        [InverseProperty("IdGrafuNavigation")]
        public virtual ICollection<GrafyRole> GrafyRole { get; set; }
    }
}
