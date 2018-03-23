using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    [Table("interFiles")]
    public partial class InterFiles
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("nazwa", TypeName = "varchar(50)")]
        public string Nazwa { get; set; }
        [Column("rozszerzenie", TypeName = "varchar(4)")]
        public string Rozszerzenie { get; set; }
        [Column("plik", TypeName = "image")]
        public byte[] Plik { get; set; }
        [Column("data", TypeName = "datetime")]
        public DateTime? Data { get; set; }
        [Column("tresc", TypeName = "varchar(max)")]
        public string Tresc { get; set; }
        [Column("nipy", TypeName = "varchar(150)")]
        public string Nipy { get; set; }
    }
}
