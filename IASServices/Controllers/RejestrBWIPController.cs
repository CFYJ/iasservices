using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using IASServices.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using JWT;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class RejestrBWIPController : Controller
    {
        private readonly RejestrBWIPContext hdcontext;

        public RejestrBWIPController(RejestrBWIPContext context)
        {
            this.hdcontext = context;
        }


        //public async Task<IEnumerable<HelpDeskInfo>> GetRows()
        [HttpGet]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetRows()
        {
            //var r = Request; 
            //var rez =  JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            //int pagesize, pagenum, recordstartindex = 0;

            //int.TryParse(r.Query["pagesize"], out pagesize);
            //int.TryParse(r.Query["pagenum"], out pagenum);
            //int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            //int startrow = recordstartindex;
            //int endrow = recordstartindex + pagesize;


            //string conditions =  FilterClass.getFilters(r.Query);


            ////var lista = await hdcontext.HelpDeskInfo.ToListAsync();

            //string query = "select id, tresc, data, zgloszenie,nr,temat,zglaszajacy,datarejestracji,status from(" +
            //    "select * from(" +
            //    "select id,data,tresc, zgloszenie,nr,temat,zglaszajacy,datarejestracji,status ,ROW_NUMBER() OVER(ORDER BY id asc) AS Row from helpdeskinfo" + conditions +
            //    ") as p1 where row between " + startrow + " and " + endrow +
            //    ")as zz";
            //var lista = await hdcontext.HelpDeskInfo.FromSql(query).ToListAsync();




            //var res = new
            //{
            //    TotalRows =  hdcontext.HelpDeskInfo.FromSql("select id, data, tresc, zgloszenie,nr,temat,zglaszajacy,datarejestracji,status from helpdeskinfo" + conditions ).Count(),
            //    Rows = lista
            //};

            //var wynik = Json(res);

            //return wynik;


            return Json("test odpowiedzi");


        }

        //[HttpGet("{searchstring}")]
        //public async Task<IEnumerable<HelpDeskInfo>> GetRowsByContent(string searchstring)
        //{
        //    //var lista = await hdcontext.HelpDeskInfo.Where(r => r.Tresc.Contains(searchstring)).ToListAsync();

        //    //return lista;
        //}

    }

 

}