using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    [Table("KontaktyJednostkiKAS")]
    public partial class KontaktyJednostkiKas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("wojewodztwo", TypeName = "varchar(50)")]
        public string Wojewodztwo { get; set; }
        [Column("typ", TypeName = "varchar(50)")]
        public string Typ { get; set; }
        [Column("nazwa_urzedu", TypeName = "varchar(150)")]
        public string NazwaUrzedu { get; set; }
        [Column("kod_pocztowy", TypeName = "varchar(10)")]
        public string KodPocztowy { get; set; }
        [Column("miasto", TypeName = "varchar(50)")]
        public string Miasto { get; set; }
        [Column("ulica", TypeName = "varchar(50)")]
        public string Ulica { get; set; }
        [Column("nr_lokalu", TypeName = "varchar(50)")]
        public string NrLokalu { get; set; }
        [Column("poczta", TypeName = "varchar(50)")]
        public string Poczta { get; set; }
        [Column("telefon", TypeName = "varchar(150)")]
        public string Telefon { get; set; }
        [Column("fax", TypeName = "varchar(150)")]
        public string Fax { get; set; }
        [Column("email", TypeName = "varchar(50)")]
        public string Email { get; set; }
        [Column("kod_jednostki", TypeName = "varchar(10)")]
        public string KodJednostki { get; set; }
        [Column("lat", TypeName = "varchar(20)")]
        public string Lat { get; set; }
        [Column("lng", TypeName = "varchar(20)")]
        public string Lng { get; set; }
    }
}
