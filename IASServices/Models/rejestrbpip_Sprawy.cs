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
        [Column("od_kogo", TypeName = "varchar(50)")]
        public string OdKogo { get; set; }
        [Column("do_kogo", TypeName = "varchar(50)")]
        public string DoKogo { get; set; }

        [Column("data_pierwszego_wniosku", TypeName = "datetime")]
        public DateTime? DataPierwszegoWniosku { get; set; }
        [Column("data_ostatniego_wniosku", TypeName = "datetime")]
        public DateTime? DataOstatniegoWniosku { get; set; }
        [Column("urzad", TypeName = "varchar(150)")]
        public string Urzad { get; set; }
        [Column("rodz_wniosku", TypeName = "varchar(50)")]
        public string RodzWniosku { get; set; }
        [Column("rodz_naleznosci", TypeName = "varchar(50)")]      
        public string RodzNaleznosci { get; set; }
        [Column("nr_bwip", TypeName = "varchar(50)")]
        public string NrBwip { get; set; }
        [Column("nr_szd", TypeName = "varchar(50)")]
        public string NrSzd { get; set; }
        [Column("calkowita_kwota", TypeName = "float")]
        public double CalkowitaKwota { get; set; }

        [Column("status", TypeName = "bit")]
        public bool? Status { get; set; }
        [Column("data_zakonczenia", TypeName = "datetime")]
        public DateTime? DataZakonczenia { get; set; }


    }
}
