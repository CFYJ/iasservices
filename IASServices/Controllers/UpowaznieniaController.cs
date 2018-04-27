//using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IASServices.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System;
using System.Text;

using System.Configuration;
using Newtonsoft.Json;


namespace IASServices.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class UpowaznieniaController : Controller
    {
        private readonly UpowaznieniaContext _context;
        private KontaktyContext _kontaktycontext;

        public UpowaznieniaController(UpowaznieniaContext context,  KontaktyContext _kcontext)
        {
            _context = context;
            _kontaktycontext = _kcontext;

            //int? zz = HttpContext.Session.GetInt32("zz");
            //HttpContext.Session.SetInt32("zz", zz.GetValueOrDefault(0) + 1);

        }

        [HttpGet]
        //Task<IEnumerable<Upowaznienia>> 
        public async Task<IActionResult> GetUpowaznieniaLista()
        {

            //var zapytanie = Request.Query["pagesize"];
            ////var queryString = HttpContext.Request.Query;
            ////return _context.Upowaznienia.FromSql("select top 3 * from upowaznienia").ToList();
            //var lista = await _context.Upowaznienia.Include(pliki =>pliki.UpowaznieniaPliki).ToListAsync();

            //return lista;
            //******************************************

            var r = Request;

            int pagesize, pagenum, recordstartindex = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            int startrow = recordstartindex;
            int endrow = recordstartindex + pagesize;


            string conditions = FilterClass.getFilters(r.Query);


            //var lista = await hdcontext.HelpDeskInfo.ToListAsync();

            string query = "select id, nazwa,nazwa_skrocona,wniosek_nadania_upr" +
                ",nadajacy_upr,prowadzacy_rejestr_uzyt,wniosek_odebrania_upr, " +
                "odbierajacy_upr,opiekun,adres_email,decyzja,uwagi  from(" +
                "select * from(" +
                "select *,ROW_NUMBER() OVER(ORDER BY id asc) AS Row from upowaznienia" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz";
            var lista = await _context.Upowaznienia.FromSql(query).Include(pliki => pliki.UpowaznieniaPliki).ToListAsync();




            var res = new
            {
                TotalRows = _context.Upowaznienia.FromSql("select * from upowaznienia" + conditions).Count(),
                Rows = lista
            };

            var wynik = Json(res);

            return wynik;

        }


        [HttpGet]
        public async Task<IActionResult> GetUpowaznieniaListaPaged()
        {

            int pagesize = int.Parse(Request.Query["pagesize"]);
            int pagenum = int.Parse(Request.Query["pagenum"]);
            int startindex = int.Parse(Request.Query["recordstartindex"]);
            //var queryString = HttpContext.Request.Query;
            //return _context.Upowaznienia.FromSql("select top 3 * from upowaznienia").ToList();


            //var lista = await _context.Upowaznienia.Include(pliki => pliki.UpowaznieniaPliki).ToListAsync();
            var lista = await _context.Upowaznienia.FromSql("select id, nazwa,nazwa_skrocona,wniosek_nadania_upr" +
                ",nadajacy_upr,prowadzacy_rejestr_uzyt,wniosek_odebrania_upr, " +
                "odbierajacy_upr,opiekun,adres_email,decyzja,uwagi " +
                "from (select *, ROW_NUMBER() OVER(ORDER BY id asc) AS Row from upowaznienia ) zz " +
                "where zz.row>="+startindex+" and zz.row<"+(startindex+pagesize)).Include(pliki => pliki.UpowaznieniaPliki).ToListAsync();



            var res = new
            {
                TotalRows = 30,
                Rows = lista
            };



            var wynik = Json(res);

            return wynik;



            #region stare







            //var lista = _context.Upowaznienia.Include(pliki => pliki.UpowaznieniaPliki).ToListAsync();
            //var res = new
            //{
            //    TotalRows = 30,
            //    Rows = lista
            //};

            //var zz = Json(res);

            //return zz;





            //var zz = JsonConvert.SerializeObject(res, new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling =
            //        Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //});

            // return Content(zz);





            // JsonConvert.SerializeObject(lista)
            //var result=new JqwidgetGridSource(30, lista);
            //var zz = JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling =
            //        Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //});

            //return Content(zz);
            // return new JsonResult(result);


            //proby







            #endregion


        }

        private class JqwidgetGridSource {
            public int TotalRows;
            public IEnumerable<Upowaznienia> Rows;

            public JqwidgetGridSource(int RowCount, IEnumerable<Upowaznienia> Rows)
            {
                this.TotalRows = RowCount;
                this.Rows = Rows;
            }

        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetUpowaznienia([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var upowaznienia = await _context.Upowaznienia.SingleOrDefaultAsync(m => m.Id == id);

            if (upowaznienia == null)
            {
                return NotFound();
            }

            return Ok(upowaznienia);
        }

    
        [HttpPost("{id}")]
        public async Task<IEnumerable<UpowaznieniaPliki>> GetPliki([FromRoute] long id)
        {
           
            var pliki = await _context.UpowaznieniaPliki.Where(m => m.IdUpowaznienia.Equals(id)).ToListAsync();
            return pliki;
         
        }

        // POST: api/Kontakties
        [HttpPost]
        public async Task<IActionResult> AddUpowaznienia([FromBody] Upowaznienia upowaznienia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<UpowaznieniaPliki> pliki = upowaznienia.UpowaznieniaPliki.ToList();
            upowaznienia.UpowaznieniaPliki.Clear();
            _context.Upowaznienia.Add(upowaznienia);
           int newid= await _context.SaveChangesAsync();


            foreach(UpowaznieniaPliki plik in pliki)
            {
                _context.Database.ExecuteSqlCommand("update upowaznieniapliki set id_upowaznienia = {0} where id={1}", upowaznienia.Id, plik.Id);
                _context.SaveChanges();
            }

            return CreatedAtAction("GetUpowaznienia", new { id = upowaznienia.Id }, upowaznienia);
        }


        // PUT: api/Kontakties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUpowaznienia([FromRoute] long id, [FromBody] Upowaznienia upowaznienia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != upowaznienia.Id)
            {
                return BadRequest();
            }

            foreach (UpowaznieniaPliki plik in upowaznienia.UpowaznieniaPliki)
            {
                _context.Database.ExecuteSqlCommand("update upowaznieniapliki set id_upowaznienia = {0} where id={1}", upowaznienia.Id, plik.Id);
                _context.SaveChanges();
            }

            _context.Entry(upowaznienia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UpowaznieniaExists(id))
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

        [HttpPost("{id}")]
        public async Task<IActionResult> DelUpowaznienia([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           // _context.Database.SqlQuery("df");
            var upowaznienia = await _context.Upowaznienia.Include(pliki => pliki.UpowaznieniaPliki).SingleOrDefaultAsync(m => m.Id == id);
            if (upowaznienia == null)
            {
                return NotFound();
            }

            foreach(UpowaznieniaPliki plik in upowaznienia.UpowaznieniaPliki)
            {

                string filePath = ConfigurationManager.AppSettings.Get("filesCatalogPath");
                var path = Path.Combine(
                               filePath,
                               plik.IdPliku);
                System.IO.File.Delete(path);
            }

            _context.Upowaznienia.Remove(upowaznienia);
            await _context.SaveChangesAsync();

            return Ok(upowaznienia);
        }


        private bool UpowaznieniaExists(long id)
           => _context.Upowaznienia.Any(e => e.Id == id);


        [HttpPost]        
        public async Task<IActionResult> FileUpload(IList<IFormFile> filess)
        {//IFormFile file
         //IList<IFormFile> files
         //[FromBody]  IFormFile fil
            var files = Request.Form.Files;

            string filePath = ConfigurationManager.AppSettings.Get("filesCatalogPath");

            foreach (IFormFile file in files)
            {

                
                if (file == null || file.Length == 0)
                    return Content("file not selected");

                string vFileId = DateTime.Now.Ticks.ToString();

                var path = Path.Combine(
                            filePath,
                            vFileId);

                try
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    UpowaznieniaPliki plik = new UpowaznieniaPliki() { IdPliku = vFileId, Nazwa = file.FileName };
                    _context.UpowaznieniaPliki.Add(plik);
                    int newid = await _context.SaveChangesAsync();

                    return CreatedAtAction("GetPliki", new { id = plik.Id }, plik);

                }
                catch(Exception ex) { return BadRequest(); }

            }
    
           // return RedirectToAction("Files");

            return Ok("true");
        }


        #region testy pobierania plikow

        /* Pobieranie plikow przez httpresponsemessage
         * application/pdf
         * application/octet-stream
         * octet/stream
         * text/plain
         * application/octet-binary
         * 
         * 
        
         
        [HttpGet("{id}")]
        //[Route("values/download")]
        public HttpResponseMessage TestDownloadd([FromRoute] int id)
            //d(string name)
        {
            try
            {
                //string fileName = string.Empty;
                //if (name.Equals("pdf", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    fileName = "SamplePdf.pdf";
                //}
                //else if (name.Equals("zip", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    fileName = "SampleZip.zip";
                //}
                string fileName = "zzz.txt";
                if (id == 1)
                    fileName = "plik.pdf";

                if (!string.IsNullOrEmpty(fileName))
                {
                    string filePath = "c:\\tmp\\"+fileName;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);

                            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            return httpResponseMessage;
                            
                        }
                    }
                }
                return (new HttpResponseMessage((HttpStatusCode.NotFound)));
            }
            catch (Exception ex)
            {
                return (new HttpResponseMessage((HttpStatusCode.InternalServerError)));
                    //this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

     */
        #endregion

     
        // ActionResult
        [HttpGet("{idpliku}")]
        public FileResult FileDownload([FromRoute] string idpliku)
        {
            string filePath = ConfigurationManager.AppSettings.Get("filesCatalogPath");    

            UpowaznieniaPliki plik = _context.UpowaznieniaPliki.Where(p => p.IdPliku.Equals(idpliku)).First();


            var path = Path.Combine(filePath, plik.IdPliku);

            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, plik.Nazwa);
            }
            catch (Exception ex) { return null; };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteFile([FromRoute] long id)
        {

            UpowaznieniaPliki plik = _context.UpowaznieniaPliki.Where(p => p.Id.Equals(id)).First();

            if (plik == null)
                return Ok(false);


            string filePath = ConfigurationManager.AppSettings.Get("filesCatalogPath");
            var path = Path.Combine(
                           filePath,
                           plik.IdPliku);
            try
            {
                System.IO.File.Delete(path);

                _context.UpowaznieniaPliki.Remove(plik);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return Ok(false);
            }

            return Ok(true);
        }


        [HttpGet]
        public async Task<IEnumerable<Telefony>> GetTelefony()
        {
            //using (var context = new SampleContext())
            //using (var command = context.Database.GetDbConnection().CreateCommand())
            //{
            //    command.CommandText = "SELECT * From Table1";
            //    context.Database.OpenConnection();
            //    using (var result = command.ExecuteReader())
            //    {
            //        // do something with result
            //    }
            //}
            using(var cxt =_kontaktycontext)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = " select imie+' ' + nazwisko as osoba, telefon from kontakty where charindex(imie + ' ' + nazwisko"
                                    + ", (SELECT  STUFF("
                                    + "(SELECT ', ' + CONVERT(NVARCHAR(max), opiekun) "
                                    + "FROM ias.dbo.upowaznienia "
                                    + "FOR xml path('') "
                                    + ")"
                                    + ", 1"
                                    + ", 1"
                                    + ", '')) ) <> 0";
                //cmd.CommandText = "select 'zzz'";
                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {                 
                    
                    List<Telefony> rez = new List<Telefony>();

                    while (result.Read())
                    {
                        //var z = result.GetValue(0);
                        rez.Add(new Telefony() { user = result.GetValue(0).ToString(), telefon = result.GetValue(1).ToString() });
                    }

                    return rez;

                }
                
            }
                //return null;
        }

        public class Telefony
        {
            public string user;
            public string telefon;

        }


    }

   
}