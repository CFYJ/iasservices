using System;
using System.Collections.Generic;

namespace IASServices.Models
{
    public partial class Roleuzytkownika
    {
        public long Id { get; set; }
        public long? IdRoli { get; set; }
        public long? IdUzytkownika { get; set; }
        public DateTime DataPoczatku { get; set; }
        public DateTime? DataKonca { get; set; }
        public string Nadal { get; set; }
        public string Odebral { get; set; }
    }
}
