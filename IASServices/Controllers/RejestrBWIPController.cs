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
using System.Configuration;
using System.IO;

using System.ComponentModel.DataAnnotations.Schema;

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


            string sortorder = " ORDER BY id desc";
     
            if (r.Query["sortorder"] != "")
                foreach (var prop in hdcontext.Model.FindEntityType(typeof(Sprawy)).GetProperties())
                {
                    if (prop.Name.ToString().ToUpper() == r.Query["sortdatafield"].ToString().ToUpper())
                        sortorder = " ORDER BY " + prop.Relational().ColumnName + " " + r.Query["sortorder"];
                }



            int startrow = recordstartindex;
            int endrow = recordstartindex + pagesize;


            string conditions = FilterClass.getFilters(r.Query);

            string query = "select * from(" +
                "select * from(" +
                "select * ,ROW_NUMBER() OVER(" + sortorder + ") AS Row from rejestr_bwip.sprawy" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz " + sortorder;
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

        [HttpPost]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> AddSprawy([FromBody] Sprawy sprawy)
        {
            var z = Request;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

               this.hdcontext.Sprawy.Add(sprawy);
            await hdcontext.SaveChangesAsync();

            return CreatedAtAction("GetSprawy", new { id = sprawy.Id }, sprawy);
        }

        private bool SprawyExists(long id)
       => hdcontext.Sprawy.Any(e => e.Id == id);



        [HttpPost("{id}")]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> DeleteSprawy([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // _context.Database.SqlQuery("df");
            var sprawy = await hdcontext.Sprawy.SingleOrDefaultAsync(m => m.Id == id);
            if (sprawy == null)
            {
                return NotFound();
            }

            hdcontext.Sprawy.Remove(sprawy);
            await hdcontext.SaveChangesAsync();

            return Ok(sprawy);
        }



        //[HttpPost]
        //[Authorize(Roles = "rejestr-bwip")]
        //public async Task<IActionResult> FileUpload(IList<IFormFile> filess)
        //{//IFormFile file
        // //IList<IFormFile> files
        // //[FromBody]  IFormFile fil
        //    var files = Request.Form.Files;

        //    string filePath = ConfigurationManager.AppSettings.Get("filesCatalogPath");

        //    foreach (IFormFile file in files)
        //    {


        //        if (file == null || file.Length == 0)
        //            return Content("file not selected");

        //        string vFileId = DateTime.Now.Ticks.ToString();

        //        var path = Path.Combine(
        //                    filePath,
        //                    vFileId);

        //        try
        //        {
        //            using (var stream = new FileStream(path, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }
        //            UpowaznieniaPliki plik = new UpowaznieniaPliki() { IdPliku = vFileId, Nazwa = file.FileName };
        //            _context.UpowaznieniaPliki.Add(plik);
        //            int newid = await _context.SaveChangesAsync();

        //            return CreatedAtAction("GetPliki", new { id = plik.Id }, plik);

        //        }
        //        catch (Exception ex) { return BadRequest(); }

        //    }

        //    // return RedirectToAction("Files");

        //    return Ok("true");
        //}

    }



}