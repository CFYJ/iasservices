using System;
using System.Collections.Generic;

namespace IASServices.Models
{
    public partial class Kontakty
    {
        public long Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Jednostka { get; set; }
        public string Miejsce_pracy { get; set; }
        public string Pion { get; set; }
        public string Wydzial { get; set; }
        public string Wydzial_podlegly { get; set; }
        public string Stanowisko { get; set; }
        public string Pokoj { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Komorka { get; set; }
        public string Login { get; set; }
        public string Wewnetrzny { get; set; }
    }
}
