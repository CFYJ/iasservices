using System;
using System.Collections.Generic;


namespace IASServices.Models
{
    public partial class Upowaznienia
    {
        public long Id { get; set; }
        public string Nazwa { get; set; }
        public string Nazwa_skrocona { get; set; }
        public string Wniosek_nadania_upr { get; set; }
        public string Nadajacy_upr { get; set; }
        public string Prowadzacy_rejestr_uzyt { get; set; }
        public string Wniosek_odebrania_upr { get; set; }
        public string Odbierajacy_upr { get; set; }
        public string Opiekun { get; set; }
        public string Adres_email { get; set; }
        public string Decyzja { get; set; }
        public string Uwagi { get; set; }


    }

}
