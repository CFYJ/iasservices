using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    [Table("pliki", Schema = "rejestr_bwip")]
    public partial class Pliki
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("id_zdarzenia")]
        public long? IdZdarzenia { get; set; }
        [Column("nazwa", TypeName = "varchar(150)")]
        public string Nazwa { get; set; }
        [Column("typ", TypeName = "varchar(4)")]
        public string Typ { get; set; }
        [Column("dane", TypeName = "image")]
        public byte[] Dane { get; set; }
        [Column("sysdate", TypeName = "datetime")]
        public DateTime Sysdate { get; set; }
        [Column("status")]
        public bool Status { get; set; }
    }
}
