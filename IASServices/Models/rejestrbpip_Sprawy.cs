using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    [Table("sprawy", Schema = "rejestr_bwip")]
    public partial class Sprawy
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("sysdate", TypeName = "datetime")]
        public DateTime? Sysdate { get; set; }
        [Column("nazwa", TypeName = "varchar(250)")]
        public string Nazwa { get; set; }
        [Column("identyfikator", TypeName = "varchar(50)")]
        public string Identyfikator { get; set; }
        [Column("adres", TypeName = "varchar(max)")]
        public string Adres { get; set; }
        [Column("data_wniosku", TypeName = "datetime")]
        public DateTime? DataWniosku { get; set; }
        [Column("od_kogo", TypeName = "varchar(50)")]
        public string OdKogo { get; set; }
        [Column("do_kogo", TypeName = "varchar(50)")]
        public string DoKogo { get; set; }
        [Column("typ", TypeName = "varchar(50)")]
        public string Typ { get; set; }
        [Column("rodz_wniosku", TypeName = "varchar(50)")]
        public string RodzWniosku { get; set; }
        [Column("nr_bwip", TypeName = "varchar(50)")]
        public string NrBwip { get; set; }
        [Column("nr_szd", TypeName = "varchar(50)")]
        public string NrSzd { get; set; }
        [Column("rodz_naleznosci", TypeName = "varchar(50)")]
        public string RodzNaleznosci { get; set; }
        [Column("calkowita_kwota", TypeName = "varchar(50)")]
        public string CalkowitaKwota { get; set; }
        [Column("termin_odpowiedzi", TypeName = "datetime")]
        public DateTime? TerminOdpowiedzi { get; set; }
    }
}
