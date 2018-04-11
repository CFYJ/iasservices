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
            var lista = await _grafycontext.GrafyGraf.Where(z => z.GrafyRole.Where(r => r.User == user).First() != null).ToListAsync();

            return lista;


        }

        //[HttpGet("{id}")]
        //public async Task<IEnumerable<GrafyRole>> GetUsersGrafsTree([FromRoute] long id)
        //{
        //    var lista = await _grafycontext.GrafyRole.ToListAsync();

        //    return lista;
        //}

        [HttpGet("{user}")]
        public async Task<IEnumerable<GrafyRole>> GetUsersGrafsTree([FromRoute] string user)
        {
            var lista = await _grafycontext.GrafyRole.Where(gr=>gr.User==user).ToListAsync();

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
            var lista = await _grafycontext.GrafyGraf.Where(z => z.Id == id).FirstOrDefaultAsync();

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
            if (rola.Typ == "katalog")
            {
                _grafycontext.GrafyRole.Add(new GrafyRole()
                {
                    IdGrafu = null,
                    IdParent = rola.IdParent,
                    Nazwa = rola.Nazwa,
                    Typ = rola.Typ,
                    Role = rola.Role,
                    User = rola.User.ToUpper(),
                });
                await _grafycontext.SaveChangesAsync();

                return rola.Id;
            }
            else
            {
                if (rola.IdGrafu == null || rola.IdGrafu == 0)
                {
                    GrafyGraf tmpgr = new GrafyGraf() { DataUtworzenia = DateTime.Now, Nazwa = rola.Nazwa };
                    _grafycontext.GrafyGraf.Add(tmpgr);
                    await _grafycontext.SaveChangesAsync();

                    rola.IdGrafu = tmpgr.Id;
                }
                rola.User = rola.User.ToUpper();
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

        [HttpPost("{id}")]
        //public async Task<IActionResult> DelGraf([FromBody] GrafyRole rola)
        public async Task<IActionResult> DelGraf(long id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return null;
            //}

            GrafyRole rola = _grafycontext.GrafyRole.Where(r => r.Id == id).FirstOrDefault();

            if (rola != null)
            {

                _grafycontext.GrafyRole.Remove(rola);

                if (rola.Typ == "katalog")
                {
                    var lista = _grafycontext.GrafyRole.Where(r => r.IdParent == rola.Id).ToList();
                    foreach (GrafyRole child in lista)
                        await DelGraf(child.Id);
                }
                else
                {
                    if (_grafycontext.GrafyRole.Where(r => r.IdGrafu == rola.IdGrafu).ToList().Count == 1)
                    {
                        var tmp = await _grafycontext.GrafyGraf.Where(g => g.Id == rola.IdGrafu).FirstOrDefaultAsync();
                        if (tmp != null)
                            _grafycontext.GrafyGraf.Remove(tmp);
                    }

                }

                //if (rola.Role == "author")
                //{                
                //    var tmp = await _grafycontext.GrafyGraf.Where(g => g.Id == rola.IdGrafu).FirstOrDefaultAsync();
                //    if (tmp != null)
                //        _grafycontext.GrafyGraf.Remove(tmp);
                //}

                await _grafycontext.SaveChangesAsync();
                return Ok("true");
            }
            return null;

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



        [HttpGet]
        public async Task<IEnumerable<UserInShare>> GetUsersInShare()
        {
            long id = long.Parse(Request.Query["id"]);
            string user = Request.Query["user"];

            using (var cxt = _grafycontext)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "select g.id,g.role, coalesce(k.Imie,'')+' '+coalesce(k.Nazwisko,'') as osoba, coalesce(k.Wydzial,'') "
                                    + "from GrafyRole g left "
                                    + "join Kontakty k on g.[user] = k.Login "
                                    + "where g.id_grafu = " + id + " and g.[user]<>'" + user + "'";


                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<UserInShare> rez = new List<UserInShare>();

                    while (result.Read())
                    {
                        rez.Add(new UserInShare()
                        {
                            id = result.GetValue(0).ToString(),
                            rola = result.GetValue(1).ToString(),
                            osoba = result.GetValue(2).ToString(),
                            wydzial = result.GetValue(3).ToString(),
                        });
                    }

                    return rez;

                }

            }
        }

        [HttpGet("{part}")]
        public async Task<IEnumerable<UserToShare>> GetUsersToShare([FromRoute] string part)
        {
    
        
            using (var cxt = _grafycontext)
            using (var cmd = cxt.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "select * from( "
                                  +"select login, coalesce(Imie, '')+' ' + coalesce(Nazwisko, '') as osoba, coalesce(Wydzial, '') as wydzial from Kontakty)zz "
                                  +"where osoba like '%"+part+"%'";


                cxt.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    if (!result.HasRows)
                        return null;

                    List<UserToShare> rez = new List<UserToShare>();

                    while (result.Read())
                    {
                        rez.Add(new UserToShare()
                        {
                            id = result.GetValue(0).ToString(),                           
                            osoba = result.GetValue(1).ToString(),
                            wydzial = result.GetValue(2).ToString(),
                        });
                    }

                    return rez;

                }

            }
        }



        public class UserInShare
        {
            public string id;
            public string rola;
            public string osoba;       
            public string wydzial;
          
        }
        public class UserToShare
        {
            public string id;
            public string osoba;
            public string wydzial;
        }

    }

}