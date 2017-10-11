using System;
using System.Collections.Generic;

namespace IASServices.Models
{
    public partial class Roleuzytkownika
    {
        public long Id { get; set; }
        public long? IdRoli { get; set; }
        public long? IdUzytkownika { get; set; }
    }
}
