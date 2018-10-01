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
    //[Authorize]
    public class HelpDeskController : Controller
    {
        private readonly HelpDeskContext hdcontext;

        public HelpDeskController(HelpDeskContext context)
        {
            this.hdcontext = context;
        }


        //public async Task<IEnumerable<HelpDeskInfo>> GetRows()
        [HttpGet]
       // [Authorize]
        //[Authorize(Policy = "HelpDeskModule")]
        [Authorize(Roles = "helpdesk")]
        public async Task<IActionResult> GetRows()
        {
            var r = Request; 
            var rez =  JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            int pagesize, pagenum, recordstartindex = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            int startrow = recordstartindex+1;
            int endrow = recordstartindex + pagesize;

           
            string conditions =  FilterClass.getFilters(r.Query);


            //var lista = await hdcontext.HelpDeskInfo.ToListAsync();

            string query = "select id, tresc, data, zgloszenie,nr,temat,zglaszajacy,datarejestracji,status from(" +
                "select * from(" +
                "select id,data,tresc, zgloszenie,nr,temat,zglaszajacy,datarejestracji,status ,ROW_NUMBER() OVER(ORDER BY id asc) AS Row from helpdeskinfo" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz";
            var lista = await hdcontext.HelpDeskInfo.FromSql(query).ToListAsync();

            


            var res = new
            {
                TotalRows =  hdcontext.HelpDeskInfo.FromSql("select id, data, tresc, zgloszenie,nr,temat,zglaszajacy,datarejestracji,status from helpdeskinfo" + conditions ).Count(),
                Rows = lista
            };

            var wynik = Json(res);

            return wynik;


        }

        [HttpGet("{searchstring}")]
        public async Task<IEnumerable<HelpDeskInfo>> GetRowsByContent(string searchstring)
        {
            var lista = await hdcontext.HelpDeskInfo.Where(r => r.Tresc.Contains(searchstring)).ToListAsync();

            return lista;
        }

    }

    //public class FsilterClass
    //{
    //    string value;
    //    string field;
    //    string condition;

    //    public static string getFilters(IQueryCollection query)
    //    {

    //        int count = int.TryParse(query["filterscount"], out count) ? count : 0;

    //        string rez = "";

    //        if(count>0)
    //        for (int i = 0; i < count; i++)
    //        {
    //            rez += i > 0 ? " and " : "";
    //            FilterClass fi = new FilterClass(query["filterGroups[" + i + "][filters][0][field]"], query["filterGroups[" + i + "][filters][0][value]"]);

    //            rez += fi.getFilter();
    //        }

    //        rez = count > 0 ? " where " + rez + " " : "";


    //        return rez;
    //    }

    //    public FilterClass(string field, string value, string condition = "CONTAINS")
    //    {
    //        this.value = value;
    //        this.field = field;
    //        this.condition = condition;
    //    }

    //    public string getFilter2(Type t)
    //    {
    //        Type myType = t;//t.GetType();
    //        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

    //        foreach (PropertyInfo prop in props)
    //        {
    //            //object propValue = prop.GetValue(t, null);
    //            if (prop.Name.ToLower() == this.field.ToLower())
    //            {
    //               // prop.PropertyType
    //            }


    //        }

    //        //string result = "";

    //        //switch (typeof())
    //        //{
    //        //    case string.

    //        //}

    //        return "";
    //    }

    //    public string getFilter()
    //    {
    //        string rez = " "+field+" like '%"+value+"%' ";            
    //        return rez;
    //    } 
    //}


}