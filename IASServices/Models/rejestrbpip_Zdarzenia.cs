using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    [Table("zdarzenia", Schema = "rejestr_bwip")]
    public partial class Zdarzenia
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("id_sprawy")]
        public long? IdSprawy { get; set; }
        [Column("data_wejscia", TypeName = "datetime")]
        public DateTime? DataWejscia { get; set; }
        [Column("data_wyjscia", TypeName = "datetime")]
        public DateTime? DataWyjscia { get; set; }
        [Column("informacja", TypeName = "varchar(max)")]
        public string Informacja { get; set; }
        [Column("sysdate", TypeName = "datetime")]
        public DateTime? Sysdate { get; set; }      
        [Column("calkowita_kwota", TypeName = "float")]
        public double CalkowitaKwota { get; set; }

    }
}
