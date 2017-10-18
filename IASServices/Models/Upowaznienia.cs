using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    [Table("upowaznienia")]
    public partial class Upowaznienia
    {
        public Upowaznienia()
        {
            UpowaznieniaPliki = new HashSet<UpowaznieniaPliki>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("nazwa", TypeName = "varchar(150)")]
        public string Nazwa { get; set; }
        [Column("nazwa_skrocona", TypeName = "varchar(50)")]
        public string Nazwa_skrocona { get; set; }
        [Column("wniosek_nadania_upr", TypeName = "varchar(250)")]
        public string Wniosek_nadania_upr { get; set; }
        [Column("nadajacy_upr", TypeName = "varchar(250)")]
        public string Nadajacy_upr { get; set; }
        [Column("prowadzacy_rejestr_uzyt", TypeName = "varchar(150)")]
        public string Prowadzacy_rejestr_uzyt { get; set; }
        [Column("wniosek_odebrania_upr", TypeName = "varchar(250)")]
        public string Wniosek_odebrania_upr { get; set; }
        [Column("odbierajacy_upr", TypeName = "varchar(250)")]
        public string Odbierajacy_upr { get; set; }
        [Column("opiekun", TypeName = "varchar(250)")]
        public string Opiekun { get; set; }
        [Column("adres_email", TypeName = "varchar(150)")]
        public string Adres_email { get; set; }
        [Column("decyzja", TypeName = "varchar(250)")]
        public string Decyzja { get; set; }
        [Column("uwagi", TypeName = "text")]
        public string Uwagi { get; set; }
   
        [InverseProperty("IdUpowaznieniaNavigation")]
        public virtual ICollection<UpowaznieniaPliki> UpowaznieniaPliki { get; set; }
    }
}
