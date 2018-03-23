using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using IASServices.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Web.Http.Results;
using System.IO;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class InterpretacjeController : Controller
    {
        private readonly InterpretacjeContext datacontext;

        public InterpretacjeController(InterpretacjeContext context)
        {
            datacontext = context;
        }



        [HttpGet("{typ}/{arg}")]
        public async Task<IActionResult> GetList([FromRoute] string typ, [FromRoute] string arg)
        {
            string searchString = " "+typ+" like '%" + arg + "%'";


            string query = "select id, nazwa,'' as rozszerzenie, " +
                " null as plik, data, dbo.getSkrot('" + arg + "',tresc) as tresc, nipy from interFiles where " + searchString;

            var lista = await datacontext.InterFiles.FromSql(query).ToListAsync();

            var wynik = Json(lista);

            return wynik;
        }


        //[HttpGet("{id}")]
        //public FileResult FileDownload([FromRoute] string id)
        //{

        //    InterFiles file = datacontext.InterFiles.Where(f => f.Id == long.Parse(id)).First();

        //    try
        //    {
        //        byte[] fileBytes = file.Plik; //System.IO.File.ReadAllBytes(path);

        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.Nazwa+'.'+file.Rozszerzenie);
        //    }
        //    catch (Exception ex) { return null; };
        //}

        [HttpGet("{id}")]
        public IActionResult FileDownload([FromRoute] string id)
        {
            try
            {
                InterFiles file = datacontext.InterFiles.Where(f => f.Id == long.Parse(id)).First();

                return new FileStreamResult(new MemoryStream(file.Plik), "application/"+file.Rozszerzenie);
            }catch(Exception ex) { return null; }
        }


        [HttpGet]
        public void upload()
        {
            byte[] plik = System.IO.File.ReadAllBytes("c:\\temp\\t.txt");

            datacontext.InterFiles.Add(new InterFiles() { Nazwa = "uploaded", Data = DateTime.Now, Nipy = "1111", Rozszerzenie = "pdf", Plik = plik });
            datacontext.SaveChanges();
        }
    }
}