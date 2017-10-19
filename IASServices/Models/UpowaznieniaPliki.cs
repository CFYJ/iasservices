using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    public partial class UpowaznieniaPliki
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("id_upowaznienia")]
        public long? IdUpowaznienia { get; set; }
        [Column("id_pliku",  TypeName = "varchar(50)")]
        public string IdPliku { get; set; }
        [Column("nazwa", TypeName = "varchar(50)")]
        public string Nazwa { get; set; }

        [ForeignKey("IdUpowaznienia")]
        [InverseProperty("UpowaznieniaPliki")]
        public virtual Upowaznienia IdUpowaznieniaNavigation { get; set; }
    }
}
