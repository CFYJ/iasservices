using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IASServices.Models
{
    public partial class HelpDeskInfo
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("tresc", TypeName = "varchar(max)")]
        public string Tresc { get; set; }
        [Column("data", TypeName = "datetime")]
        public DateTime? Data { get; set; }
        [Column("zgloszenie", TypeName = "varchar(max)")]
        public string Zgloszenie { get; set; }

        [Column("nr", TypeName = "varchar(50)")]
        public string Nr { get; set; }
        [Column("temat", TypeName = "varchar(max)")]
        public string Temat { get; set; }
        [Column("zglaszajacy", TypeName = "varchar(300)")]
        public string Zglaszajacy { get; set; }
        [Column("datarejestracji", TypeName = "varchar(50)")]
        public string Datarejestracji { get; set; }
        [Column("status", TypeName = "varchar(50)")]
        public string Status { get; set; }
    }
}
