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

//using System.Data.e


namespace IASServices.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class UpowaznieniaController : Controller
    {
        private readonly UpowaznieniaContext _context;

        public UpowaznieniaController(UpowaznieniaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Upowaznienia>> GetUpowaznieniaLista()
        {
            //var queryString = HttpContext.Request.Query;
            //return _context.Upowaznienia.FromSql("select top 3 * from upowaznienia").ToList();
            var lista = await _context.Upowaznienia.Include(pliki =>pliki.UpowaznieniaPliki).ToListAsync();
            return lista;
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

            _context.Upowaznienia.Add(upowaznienia);
           int newid= await _context.SaveChangesAsync();

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
            var upowaznienia = await _context.Upowaznienia.SingleOrDefaultAsync(m => m.Id == id);
            if (upowaznienia == null)
            {
                return NotFound();
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

            foreach (IFormFile file in files)
            {
            

                if (file == null || file.Length == 0)
                    return Content("file not selected");

                var path = Path.Combine(
                            //Directory.GetCurrentDirectory(), "wwwroot",
                            "c:\\tmp",
                            file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            }
    
           // return RedirectToAction("Files");

            return Ok("true");
        }


        
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

        // ActionResult
        [HttpGet("{id}")]
        public FileResult FileDownload([FromRoute] int id)
        {

            string plik = "zzz.txt";
            if (id == 1)
                plik = "plik.pdf";

            byte[] fileBytes = System.IO.File.ReadAllBytes("c:\\tmp\\"+plik);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet,plik);
           

        }



    }
}