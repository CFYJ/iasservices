using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using IASServices.Models;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class GrafyController : Controller
    {
        private readonly GrafyContext _grafycontext;
        private readonly KontaktyContext kcontext;

        public GrafyController(GrafyContext context, KontaktyContext kcontext)
        {
            this._grafycontext = context;
            this.kcontext = kcontext;

        }

        [HttpGet]
        public async Task<IEnumerable<GrafyGraf>> GetGrafyLista(string user)
        {
            var lista = await _grafycontext.GrafyGraf.Where(z=>z.GrafyRole.Where(r=>r.User == user).First() != null).ToListAsync();

            return lista;


        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<GrafyRole>> GetUsersGrafsTree([FromRoute] long id)
        {
            var lista = await _grafycontext.GrafyRole.ToListAsync();

            return lista;
        }

        [HttpGet]
        public async Task<IEnumerable<GrafyGraf>> GetGrafyTree(string user)
        {
            var lista = await _grafycontext.GrafyGraf.Where(z => z.GrafyRole.Where(r => r.User == user).First() != null).ToListAsync();

            return lista;
        }

        

        [HttpGet("{id}")]
        public async Task<GrafyGraf> GetGraf([FromRoute] long id)
        {
            var lista = await _grafycontext.GrafyGraf.Where(z => z.Id  == id).FirstOrDefaultAsync();

            return lista;
        }

        [HttpPost]
        //public async Task<long?> AddGraf([FromBody] GrafyGraf graf, string user)
        public async Task<long?> AddGraf([FromBody] GrafyRole rola)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            if(rola.Typ == "katalog")
            {
                _grafycontext.GrafyRole.Add(new GrafyRole()
                {
                    IdGrafu =null,
                    IdParent = rola.IdParent,
                    Nazwa = rola.Nazwa,
                    Typ = rola.Typ,
                    Role = rola.Role,
                });
                await _grafycontext.SaveChangesAsync();     

                return rola.Id;
            }
            else
            {
                GrafyGraf tmpgr = new GrafyGraf() { DataUtworzenia = DateTime.Now, Nazwa = rola.Nazwa };
                _grafycontext.GrafyGraf.Add(tmpgr);
                 await _grafycontext.SaveChangesAsync();

                rola.IdGrafu = tmpgr.Id;
                _grafycontext.GrafyRole.Add(rola);
                await _grafycontext.SaveChangesAsync();


                return rola.Id;
            }

            //_grafycontext.GrafyGraf.Add(graf);
            //long newid = await _grafycontext.SaveChangesAsync();

            //_grafycontext.GrafyRole.Add(new GrafyRole() { IdGrafu = graf.Id, Role = "author", User = user, Nazwa = graf.Nazwa });
            //await _grafycontext.SaveChangesAsync();


            //return graf.Id;


        }

        [HttpPost]
        public async Task<IActionResult> DelGraf([FromBody] GrafyRole rola)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            _grafycontext.GrafyRole.Remove(rola);

            if (rola.Typ == "katalog")
            {
                var lista = _grafycontext.GrafyRole.Where(r => r.IdParent == rola.Id).ToList();
                foreach (GrafyRole child in lista)
                    await DelGraf(child);
            }

            if (rola.Role == "author")
            {
                var tmp = await _grafycontext.GrafyGraf.Where(g => g.Id == rola.IdGrafu).FirstOrDefaultAsync();
                if(tmp!=null)
                _grafycontext.GrafyGraf.Remove(tmp);   
            }

            await _grafycontext.SaveChangesAsync();
            return Ok("true");

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateGraf([FromRoute] long id, [FromBody] GrafyGraf graf)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != graf.Id)
            {
                return BadRequest();
            }

            _grafycontext.Entry(graf).State = EntityState.Modified;

            try
            {
                await _grafycontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrafyExists(id))
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

        private bool GrafyExists(long id)
         => _grafycontext.GrafyGraf.Any(e => e.Id == id);



        [HttpGet("{id}")]
        public async Task<IEnumerable<GrafyRole>> GetGrafyShares([FromRoute] long id, [FromBody] string user)
        {
            var lista = await _grafycontext.GrafyRole.Where(r => r.User == user && r.IdGrafu==id).ToListAsync();

            if (lista.Count <= 0)
                return null;

            string logins = "";
            foreach(GrafyRole g in lista)
            {
                if(g.User != user)
                logins += "'" + g.User + "',";
            }

            lista



            return lista;
        }

    }
}