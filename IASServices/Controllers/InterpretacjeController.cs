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
using Microsoft.AspNetCore.Authorization;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "interpretacje")]
    public class InterpretacjeController : Controller
    {
        private readonly InterpretacjeContext datacontext;

        public InterpretacjeController(InterpretacjeContext context)
        {
            datacontext = context;

            //logger("start");
        }



        [HttpGet("{typ}/{arg}")]
        public async Task<IActionResult> GetList([FromRoute] string typ, [FromRoute] string arg)
        {
            try
            {
                string searchString = " " + typ + " like '%" + arg + "%'";


                string query = "select id, nazwa,'' as rozszerzenie, " +
                    " null as plik, data, dbo.getSkrot('" + arg + "',tresc) as tresc, nipy from interFiles where " + searchString;

                var lista = await datacontext.InterFiles.FromSql(query).ToListAsync();

                var wynik = Json(lista);

                return wynik;
            }catch(Exception ex) { logger(ex.Message); return null; }
        }

        [HttpGet]
        public async Task<IActionResult> GetRows()
        {
            var r = Request;

            int pagesize, pagenum, recordstartindex = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            int startrow = recordstartindex;
            int endrow = recordstartindex + pagesize;


            string conditions = FilterClass.getFilters(r.Query);

            string query = "select id,nazwa,nipy,data,plik,rozszerzenie,tresc from(" +
                "select * from(" +
                "select id,nazwa,nipy,data,null as plik, null as tresc, '' as rozszerzenie ,ROW_NUMBER() OVER(ORDER BY id asc) AS Row from InterFiles" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz";
            var lista = await datacontext.InterFiles.FromSql(query).ToListAsync();




            var res = new
            {
                TotalRows = datacontext.InterFiles.FromSql("select  id,nazwa,nipy,data,null as plik, '' as tresc,'' as rozszerzenie from InterFiles" + conditions).Count(),
                Rows = lista
            };

            var wynik = Json(res);

            return wynik;


        }




        #region stary download
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



        //[HttpGet("{id}")]
        //public IActionResult FileDownload([FromRoute] string id)
        //{
        //    try
        //    {
        //        InterFiles file = datacontext.InterFiles.Where(f => f.Id == long.Parse(id)).First();

        //        return new FileStreamResult(new MemoryStream(file.Plik), "application/" + file.Rozszerzenie);
        //    }
        //    catch (Exception ex) { logger(ex.Message); return null; }
        //}

        //*******************************************************************


        //[HttpGet("{id}")]
        //public FileResult FileDownload([FromRoute] string id)
        //{
        //    InterFiles file = datacontext.InterFiles.Where(f => f.Id == long.Parse(id)).First();

        //    try
        //    {
        //        byte[] fileBytes = file.Plik;

        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.Nazwa);
        //    }
        //    catch (Exception ex) { return null; };
        //}

        //******************************************************


        //[HttpGet("{id}")]
        //public HttpResponseMessage FileDownload([FromRoute] string id)
        //{
        //    InterFiles file = datacontext.InterFiles.Where(f => f.Id == long.Parse(id)).First();

        //    var dataStream = new MemoryStream(file.Plik);

        //    HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);//Response.CreateResponse(HttpStatusCode.OK);
        //    httpResponseMessage.Content = new StreamContent(dataStream);
        //    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //    httpResponseMessage.Content.Headers.ContentDisposition.FileName = file.Nazwa + ".pdf";
        //    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        //    return httpResponseMessage;
        //}


        //*****************************************************************

        #endregion

        [HttpGet("{id}")]
        public void FileDownload([FromRoute] string id)
        {
            try
            {
                InterFiles file = datacontext.InterFiles.Where(f => f.Id == long.Parse(id)).First();

                //return new FileStreamResult(new MemoryStream(file.Plik), "application/" + file.Rozszerzenie);

                //Response.ContentType = "application/pdf";
                //Response.Headers.Add("Content-Disposition", "inline; filename=" + file.Nazwa + "." + file.Rozszerzenie);
                Response.Headers.Add("Content-Disposition", "attachment;filename=" + file.Nazwa + "." + file.Rozszerzenie);

                Response.Body.Write(file.Plik, 0, file.Plik.Length);

            }
            catch (Exception ex) { logger(ex.Message); }
        }


        [HttpGet]
        public void upload()
        {
            byte[] plik = System.IO.File.ReadAllBytes("c:\\temp\\t.txt");

            datacontext.InterFiles.Add(new InterFiles() { Nazwa = "uploaded", Data = DateTime.Now, Nipy = "1111", Rozszerzenie = "pdf", Plik = plik });
            datacontext.SaveChanges();
        }

        private void logger(string info)
        {
            System.IO.File.AppendAllText("log.txt", "\r\n\r\n\r\n*********************** " + DateTime.Now.ToLongDateString());
            System.IO.File.AppendAllText("log.txt", info);
        }
    }
}