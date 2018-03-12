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

        public GrafyController(GrafyContext context)
        {
            _grafycontext = context;

        }

        [HttpGet]
        public async Task<IEnumerable<GrafyGraf>> GetGrafyLista(string user)
        {
            var lista = await _grafycontext.GrafyGraf.Where(z=>z.GrafyRole.Where(r=>r.User == user).First() != null).ToListAsync();

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
        public async Task<long?> AddGraf([FromBody] GrafyGraf graf, string user)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
  
            _grafycontext.GrafyGraf.Add(graf);
            long newid = await _grafycontext.SaveChangesAsync();

            _grafycontext.GrafyRole.Add(new GrafyRole() { IdGrafu = newid, Role = "author", User = user, Nazwa = graf.Nazwa });



            return newid;
        }

    }
}