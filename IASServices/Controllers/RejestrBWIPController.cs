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
using System.Data.SqlClient;

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

        #region sprawy

        [HttpGet]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetSprawy()
        {
            var r = Request;
            var rez = JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            int pagesize, pagenum, recordstartindex, filterscount = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);
            int.TryParse(r.Query["filterscount"], out filterscount);


            string sortorder = (r.Query.Where(a => a.Key == "sortorder").Count() > 0) ? FilterClass.getSortCondition(r.Query, (typeof(Sprawy))) : " ORDER BY id desc";


            #region stare metody wyci¹gajace nazwê kolumny z bazy
            //if (r.Query["sortorder"] != "")
            //    foreach (var prop in hdcontext.Model.FindEntityType(typeof(Sprawy)).GetProperties())
            //    {
            //        if (prop.Name.ToString().ToUpper() == r.Query["sortdatafield"].ToString().ToUpper())
            //            sortorder = " ORDER BY " + prop.Relational().ColumnName + " " + r.Query["sortorder"];
            //    }



            //if (r.Query["sortorder"] != "")
            //    foreach (var prop in (typeof(Sprawy)).GetProperties())
            //    {
            //        //if (prop.Name.ToString().ToUpper() == r.Query["sortdatafield"].ToString().ToUpper())
            //        //    sortorder = " ORDER BY " + prop. .Relational().ColumnName + " " + r.Query["sortorder"];

            //        var z = prop.Name;
            //        var x = prop.GetCustomAttributes().First();
            //        string s = (x as ColumnAttribute).Name;
            //    }

            #endregion


            int startrow = recordstartindex + 1;
            int endrow = recordstartindex + pagesize;


            string conditions = FilterClass.generateQueryCondition(r.Query, null, typeof(Sprawy));

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

       

        [HttpGet("{nrBwip}")]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetSprawyByID([FromRoute] string nrBwip)
        {

            var lista = await hdcontext.Sprawy.Where(a => a.NrBwip == nrBwip).FirstOrDefaultAsync();

            var wynik = Json(lista);

            return wynik;


        }

        [HttpGet]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetSprawyStare()
        {

            var lista = await hdcontext.Sprawy.Where(a => a.DataOstatniegoWniosku< DateTime.Now.AddMonths(-6)).CountAsync();

            var wynik = Json(lista);

            return wynik;


        }




        [HttpPut("{id}")]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> UpdateSprawy([FromRoute] long id, [FromBody] Sprawy sprawy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sprawy.Id)
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

            //return Ok(sprawy);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> AddSprawy([FromBody] Sprawy sprawy)
        {
            var z = Request;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(sprawy.DataPierwszegoWniosku != null)
            {
                sprawy.DataOstatniegoWniosku = sprawy.DataPierwszegoWniosku;
            }
            sprawy.Sysdate = DateTime.Now;
            this.hdcontext.Sprawy.Add(sprawy);
            await hdcontext.SaveChangesAsync();

            return CreatedAtAction("GetSprawy", new { id = sprawy.Id }, sprawy);
        }


        [HttpPost("{id}")]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
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

        private bool SprawyExists(long id)
       => hdcontext.Sprawy.Any(e => e.Id == id);



        #endregion

        #region zdarzenia

        [HttpGet]
        [Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetZdarzenia()
        {
            var r = Request;
            //var rez = JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            int pagesize, pagenum, recordstartindex = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            int startrow = recordstartindex + 1;
            int endrow = recordstartindex + pagesize;

            int id = 0;
            int.TryParse(Request.Query["id"], out id);

            string sortorder = (r.Query.Where(a => a.Key == "sortorder").Count() > 0) ? FilterClass.getSortCondition(r.Query, (typeof(Zdarzenia))) : " ORDER BY id desc";

            string conditions = FilterClass.generateQueryCondition(r.Query, null, typeof(Zdarzenia)) + " and id_sprawy=" + id.ToString();



            string query = "select * from(" +
                "select * from(" +
                "select * ,ROW_NUMBER() OVER(" + sortorder + ") AS Row from rejestr_bwip.zdarzenia" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz " + sortorder;
            var lista = await hdcontext.Zdarzenia.FromSql(query).ToListAsync();


            var res = new
            {
                TotalRows = hdcontext.Zdarzenia.FromSql("select * from rejestr_bwip.zdarzenia" + conditions).Count(),
                Rows = lista
            };

            var wynik = Json(res);

            return wynik;


        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "rejestr-bwip")]
        public async Task<IActionResult> GetZdarzenia([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var upowaznienia = await hdcontext.Zdarzenia.SingleOrDefaultAsync(m => m.Id == id);

            if (upowaznienia == null)
            {
                return NotFound();
            }

            return Ok(upowaznienia);
        }

        [HttpPost]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> AddZdarzenia([FromBody] Zdarzenia zdarzenia)
        {
            var z = Request;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            zdarzenia.Sysdate = DateTime.Now;
            this.hdcontext.Zdarzenia.Add(zdarzenia);

            await hdcontext.SaveChangesAsync();

            var spr = await hdcontext.Sprawy.Where(s => s.Id == zdarzenia.IdSprawy).FirstOrDefaultAsync();
            if (zdarzenia.DataWejscia != null)
                if (zdarzenia.DataWejscia > spr.DataOstatniegoWniosku || spr.DataOstatniegoWniosku == null)                
                    spr.DataOstatniegoWniosku = zdarzenia.DataWejscia;
                
            spr.CalkowitaKwota = zdarzenia.CalkowitaKwota;
            hdcontext.Entry(spr).State = EntityState.Modified;
            await hdcontext.SaveChangesAsync();

            return CreatedAtAction("GetZdarzenia", new { id = zdarzenia.Id }, zdarzenia);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> UpdateZdarzenia([FromRoute] long id, [FromBody] Zdarzenia zdarzenia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != zdarzenia.Id)
            {
                return BadRequest();
            }




            hdcontext.Entry(zdarzenia).State = EntityState.Modified;

            try
            {
                await hdcontext.SaveChangesAsync();



                var spr = await hdcontext.Sprawy.Where(s => s.Id == zdarzenia.IdSprawy).FirstOrDefaultAsync();
                if (zdarzenia.DataWejscia != null)
                    if (zdarzenia.DataWejscia > spr.DataOstatniegoWniosku || spr.DataOstatniegoWniosku == null)
                    {
                        spr.DataOstatniegoWniosku = zdarzenia.DataWejscia;
                        hdcontext.Entry(spr).State = EntityState.Modified;
                        await hdcontext.SaveChangesAsync();
                    }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZdarzeniaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
            //return Ok(zdarzenia);
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> DeleteZdarzenia([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // _context.Database.SqlQuery("df");
            var zdarzenia = await hdcontext.Zdarzenia.SingleOrDefaultAsync(m => m.Id == id);
            if (zdarzenia == null)
                return NotFound();


            hdcontext.Zdarzenia.Remove(zdarzenia);
            await hdcontext.SaveChangesAsync();



            return Ok(zdarzenia);
        }


        private bool ZdarzeniaExists(long id) => hdcontext.Zdarzenia.Any(e => e.Id == id);

        #endregion


        #region pliki

        [HttpGet]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> GetPliki()
        {
            var r = Request;
            //var rez = JsonWebToken.Decode(Request.Headers["Authorization"], "VeryCompl!c@teSecretKey", false);

            //int pagesize, pagenum, recordstartindex = 0;

            //int.TryParse(r.Query["pagesize"], out pagesize);
            //int.TryParse(r.Query["pagenum"], out pagenum);
            //int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            //int startrow = recordstartindex+1;
            //int endrow = recordstartindex + pagesize;


            //string conditions = FilterClass.getFilters(r.Query);


            int id = 0;
            int.TryParse(Request.Query["id"], out id);


            string query = "select id, id_zdarzenia, nazwa, typ, null as dane, sysdate, status  from rejestr_bwip.pliki where id_zdarzenia=" + id+" order by id desc";
            var lista = await hdcontext.Pliki.FromSql(query).ToListAsync();


            //var res = new
            //{
            //    TotalRows = hdcontext.Pliki.Where(a => a.IdZdarzenia==id).Count(),
            //    Rows = lista
            //};

            var wynik = Json(lista);

            return wynik;


        }


        [HttpPost]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> AddPliki(IList<IFormFile> filess)
        {

            var files = Request.Form.Files;

            foreach (IFormFile file in files)
            {


                if (file == null || file.Length == 0)
                    return Content("file not selected");

                string typ = "";
                string[] lista;
                if ((lista = file.FileName.Split('.')).Count() > 1)
                    typ = lista[lista.Length - 1];

                try
                {
                    Pliki newfile = new Pliki() { Nazwa = file.FileName, Typ = typ };

                    using (MemoryStream ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        newfile.Dane = ms.ToArray();
                        newfile.Sysdate = DateTime.Now;
                        newfile.Status = false;

                        hdcontext.Pliki.Add(newfile);
                        int newid = await hdcontext.SaveChangesAsync();
                    }

                    return CreatedAtAction("GetPliki", new { id = newfile.Id }, newfile);

                }
                catch (Exception ex) { return BadRequest(); }

            }


            return Ok("true");
        }


        [HttpPost("{id}")]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> UploadPliki([FromRoute] long id, IList<IFormFile> filess)
        {
            var files = Request.Form.Files;

            foreach (IFormFile file in files)
            {
                if (file == null || file.Length == 0)
                    return Content("file not selected");

                string typ = "";
                string[] lista;
                if ((lista = file.FileName.Split('.')).Count() > 1)
                    typ = lista[lista.Length - 1];

                try
                {
                    Pliki newfile = new Pliki() { Nazwa = file.FileName, Typ = typ.Length > 3 ? typ.Substring(0, 3) : typ };

                    using (MemoryStream ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        newfile.Dane = ms.ToArray();
                        newfile.Sysdate = DateTime.Now;
                        newfile.Status = true;

                        newfile.IdZdarzenia = id;
                        hdcontext.Pliki.Add(newfile);
                        int newid = await hdcontext.SaveChangesAsync();
                    }

                    return CreatedAtAction("GetPliki", new { id = newfile.Id }, newfile);

                }
                catch (Exception ex) {
                    return BadRequest();
                }

            }


            return Ok("true");

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> UpdatePliki([FromRoute] long id, [FromBody] Pliki pliki)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pliki.Id)
            {
                return BadRequest();
            }
            pliki.Status = true;
            hdcontext.Entry(pliki).State = EntityState.Modified;

            try
            {
                await hdcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlikiExists(id))
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

        private bool PlikiExists(long id) => hdcontext.Pliki.Any(e => e.Id == id);

        [HttpPost("{id}")]
        [Authorize(Roles = "rejestr-bwip,rejestr-bwip-edycja")]
        public async Task<IActionResult> DeletePliki([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // _context.Database.SqlQuery("df");


            //var pliki = await hdcontext.Pliki.SingleOrDefaultAsync(m => m.Id == id);
            //if (pliki == null)
            //    return NotFound();


            //hdcontext.Pliki.Remove(pliki);
            //await hdcontext.SaveChangesAsync();

            //return Ok(pliki);

            hdcontext.Database.ExecuteSqlCommand("delete from rejestr_bwip.pliki where id=@ID", new SqlParameter("@ID", id));

            return Ok("true");

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "rejestr-bwip")]
        public void DownloadPliki([FromRoute] string id)
        {
            try
            {
                Pliki file = hdcontext.Pliki.Where(f => f.Id == long.Parse(id)).First();

                Response.Headers.Add("Content-Disposition", "attachment;filename=" + file.Nazwa);

                Response.Body.Write(file.Dane, 0, file.Dane.Length);

            }
            catch (Exception ex) { Response.StatusCode = 500; }
        }

        #endregion

    }
}