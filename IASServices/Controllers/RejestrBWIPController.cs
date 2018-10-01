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


        [HttpGet]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetSprawy()
        {
            var r = Request;
            var rez = JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            int pagesize, pagenum, recordstartindex = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            int startrow = recordstartindex;
            int endrow = recordstartindex + pagesize;


            string conditions = FilterClass.getFilters(r.Query);

            string query = "select * from(" +
                "select * from(" +
                "select * ,ROW_NUMBER() OVER(ORDER BY id asc) AS Row from rejestr_bwip.sprawy" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz";
            var lista = await hdcontext.Sprawy.FromSql(query).ToListAsync();        


            var res = new
            {
                TotalRows = hdcontext.Sprawy.FromSql("select * from rejestr_bwip.sprawy" + conditions).Count(),
                Rows = lista
            };

            var wynik = Json(res);

            return wynik;


        }


        [HttpGet]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetZdarzenia()
        {
            var r = Request;
            var rez = JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            int pagesize, pagenum, recordstartindex = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            int startrow = recordstartindex;
            int endrow = recordstartindex + pagesize;


            string conditions = FilterClass.getFilters(r.Query);

            string query = "select * from(" +
                "select * from(" +
                "select * ,ROW_NUMBER() OVER(ORDER BY id asc) AS Row from rejestr_bwip.zdarzenia" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz";
            var lista = await hdcontext.Zdarzenia.FromSql(query).ToListAsync();


            var res = new
            {
                TotalRows = hdcontext.Zdarzenia.FromSql("select * from rejestr_bwip.zdarzenia" + conditions).Count(),
                Rows = lista
            };

            var wynik = Json(res);

            return wynik;


        }


        [HttpPut("{id}")]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> UpdateSprawy([FromRoute] long id, [FromBody] Sprawy sprawy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id !=sprawy.Id)
            {
                return BadRequest();
            }
   

            hdcontext.Entry(sprawy).State = EntityState.Modified;

            try
            {
                await hdcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SprawyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool SprawyExists(long id)
       => hdcontext.Sprawy.Any(e => e.Id == id);


    }



}