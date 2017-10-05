using System;
using System.ComponentModel.DataAnnotations;

namespace SyncWebIAS.Model
{
    public partial class Delegacja
    {
        public long Id { get; set; }
        public long? IdWystawcy { get; set; }
        public long? IdDelegowanego { get; set; }
        public string Nr { get; set; }
        public DateTime? CzasOd { get; set; }
        public DateTime? CzasDo { get; set; }

        [Required(ErrorMessage ="Pole 'Miejscowość' jest wymagane")]
        [MinLength(3, ErrorMessage ="Minimalna długość pola 'Miejscowość' wynosi 3 znaki")]
        public string Miejscowosc { get; set; }

        public string Cel { get; set; }
        public string Srodek { get; set; }
        public DateTime? DataWystawienia { get; set; }
        public string Wystawil { get; set; }
        public string Delegowany { get; set; }
        public string Wydzial { get; set; }
    }
}
