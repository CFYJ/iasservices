using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.IO;
using Microsoft.AspNetCore.Http;
using JWT;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class AmortyzacjaController : Controller
    {

        [HttpGet]
        [Authorize(Roles = "dofinansowaniepops")]
        public async Task<IActionResult> GetRows()
        {
            string filePath = ConfigurationManager.AppSettings.Get("daneAmortyzacji");

            var r = Request;
            var rez =  JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            int pagesize, pagenum= 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);

            Amort warunek = new Amort().getFilters(r.Query);

            var reader = new StreamReader(filePath);
            var csv = new CsvReader(reader);
            var records = csv.GetRecords<Amort>();

            var readeril = new StreamReader(filePath);
            var csvil = new CsvReader(readeril);
            var recordsil = csvil.GetRecords<Amort>();

            var res = new
            {
                rows = records.Where(war => (war.b.ToUpper().Contains(warunek.b.ToUpper())) 
                            && (war.c.ToUpper().Contains(warunek.c.ToUpper()))
                            && (war.d.ToUpper().Contains(warunek.d.ToUpper()))
                            && (war.e.ToUpper().Contains(warunek.e.ToUpper()))
                            && (war.f.ToUpper().Contains(warunek.f.ToUpper()))
                            && (war.g.ToUpper().Contains(warunek.g.ToUpper()))
                            && (war.h.ToUpper().Contains(warunek.h.ToUpper()))
                            && (war.i.ToUpper().Contains(warunek.i.ToUpper()))
                            && (war.j.ToUpper().Contains(warunek.j.ToUpper()))
                            && (war.k.ToUpper().Contains(warunek.k.ToUpper()))
                            && (war.l.ToUpper().Contains(warunek.l.ToUpper()))
                            && (war.m.ToUpper().Contains(warunek.m.ToUpper()))
                            && (war.n.ToUpper().Contains(warunek.n.ToUpper()))
                            && (war.a.ToUpper().Contains(warunek.a.ToUpper())))
                        .Skip(pagesize * pagenum).Take(pagesize),
                
                TotalRows = recordsil.Where(war => (war.b.ToUpper().Contains(warunek.b.ToUpper()))
                            && (war.c.ToUpper().Contains(warunek.c.ToUpper()))
                            && (war.d.ToUpper().Contains(warunek.d.ToUpper()))
                            && (war.e.ToUpper().Contains(warunek.e.ToUpper()))
                            && (war.f.ToUpper().Contains(warunek.f.ToUpper()))
                            && (war.g.ToUpper().Contains(warunek.g.ToUpper()))
                            && (war.h.ToUpper().Contains(warunek.h.ToUpper()))
                            && (war.i.ToUpper().Contains(warunek.i.ToUpper()))
                            && (war.j.ToUpper().Contains(warunek.j.ToUpper()))
                            && (war.k.ToUpper().Contains(warunek.k.ToUpper()))
                            && (war.l.ToUpper().Contains(warunek.l.ToUpper()))
                            && (war.m.ToUpper().Contains(warunek.m.ToUpper()))
                            && (war.n.ToUpper().Contains(warunek.n.ToUpper()))
                            && (war.a.ToUpper().Contains(warunek.a.ToUpper()))).Count(),
                
            };
            var wynik = Json(res);

            return wynik;
        }
    }

    public class Amort
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public string d { get; set; }
        public string e { get; set; }
        public string f { get; set; }
        public string g { get; set; }
        public string h { get; set; }
        public string i { get; set; }
        public string j { get; set; }
        public string k { get; set; }
        public string l { get; set; }
        public string m { get; set; }
        public string n { get; set; }


        public Amort getFilters(IQueryCollection query)
        {
            Amort warunek = new Amort();
            warunek.a = ""; warunek.b = ""; warunek.c = ""; warunek.d = ""; warunek.e = ""; warunek.f = "";
            warunek.g = ""; warunek.h = ""; warunek.i = ""; warunek.j = ""; warunek.k = ""; warunek.l = ""; warunek.m = ""; warunek.n = "";
            int count = int.TryParse(query["filterscount"], out count) ? count : 0;

            if (count > 0)
                for (int i = 0; i < count; i++)
                {
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "a") { warunek.a = query["filterGroups[" + i + "][filters][0][value]"];};
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "b") { warunek.b = query["filterGroups[" + i + "][filters][0][value]"];};
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "c") {warunek.c = query["filterGroups[" + i + "][filters][0][value]"];};
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "d") { warunek.d = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "e") { warunek.e = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "f") { warunek.f = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "g") { warunek.g = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "h") { warunek.h = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "i") { warunek.i = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "j") { warunek.j = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "k") { warunek.k = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "l") { warunek.l = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "m") { warunek.m = query["filterGroups[" + i + "][filters][0][value]"]; };
                    if (query["filterGroups[" + i + "][filters][0][field]"] == "n") { warunek.n = query["filterGroups[" + i + "][filters][0][value]"]; };
                    //  rez += fi.getFilter();
                }

            
            return warunek;
        }
    }
}