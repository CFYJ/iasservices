using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IASServices.Models;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class KontaktiesController : Controller
    {
        private readonly KontaktyContext _context;

        public KontaktiesController(KontaktyContext context)
        {
            _context = context;
        }

        [HttpGet]
        //Task<IEnumerable<Kontakty>> 
        public async Task<IActionResult> GetKontakty()
        {
            
            //return await _context.Kontakty.ToListAsync();
            //***********************************************

            var r = Request;

            int pagesize, pagenum, recordstartindex = 0;

            int.TryParse(r.Query["pagesize"], out pagesize);
            int.TryParse(r.Query["pagenum"], out pagenum);
            int.TryParse(r.Query["recordstartindex"], out recordstartindex);

            int startrow = recordstartindex;
            int endrow = recordstartindex + pagesize;


            string conditions = FilterClass.getFilters(r.Query);


            //var lista = await hdcontext.HelpDeskInfo.ToListAsync();

            string query = "select id,imie, nazwisko, jednostka,miejsce_pracy,pion,wydzial,wydzial_podlegly,stanowisko,pokoj,email,telefon,komorka,login,wewnetrzny from(" +
                "select * from(" +
                "select *,ROW_NUMBER() OVER(ORDER BY id asc) AS Row from kontakty" + conditions +
                ") as p1 where row between " + startrow + " and " + endrow +
                ")as zz";
            var lista = await _context.Kontakty.FromSql(query).ToListAsync();




            var res = new
            {
                TotalRows = _context.Kontakty.FromSql("select * from kontakty" + conditions).Count(),
                Rows = lista
            };

            var wynik = Json(res);

            return wynik;
        }

        // GET: api/Kontakties/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKontakty([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kontakty = await _context.Kontakty.SingleOrDefaultAsync(m => m.Id == id);

            if (kontakty == null)
            {
                return NotFound();
            }

            return Ok(kontakty);
        }

        // PUT: api/Kontakties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKontakty([FromRoute] long id, [FromBody] Kontakty kontakty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != kontakty.Id)
            {
                return BadRequest();
            }

            _context.Entry(kontakty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KontaktyExists(id))
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

        // POST: api/Kontakties
        [HttpPost]
        public async Task<IActionResult> PostKontakty([FromBody] Kontakty kontakty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Kontakty.Add(kontakty);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKontakty", new { id = kontakty.Id }, kontakty);
        }

        // DELETE: api/Kontakties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKontakty([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kontakty = await _context.Kontakty.SingleOrDefaultAsync(m => m.Id == id);
            if (kontakty == null)
            {
                return NotFound();
            }

            _context.Kontakty.Remove(kontakty);
            await _context.SaveChangesAsync();

            return Ok(kontakty);
        }

        private bool KontaktyExists(long id)
            => _context.Kontakty.Any(e => e.Id == id);

        [HttpGet]
        public async Task<IList<string>> GetWydzial()
            => await _context.Kontakty.OrderBy(k => k.Wydzial).Select(k => k.Wydzial).Distinct().ToListAsync();

        [HttpGet]
        public async Task<IEnumerable<string>> GetWydzialPodlegly()
            => await _context.Kontakty.OrderBy(k => k.Wydzial_podlegly).Select(k => k.Wydzial_podlegly).Distinct().ToListAsync();

        [HttpGet]
        public async Task<IEnumerable<string>> GetJednostka()
        {
            return await _context.Kontakty.OrderBy(k => k.Jednostka).Select(k => k.Jednostka).Distinct().ToListAsync();
        }


        [HttpGet]
        public async Task<IEnumerable<string>> GetMiejscePracy()
            => await _context.Kontakty.OrderBy(k => k.Miejsce_pracy).Select(k => k.Miejsce_pracy).Distinct().ToListAsync();

        [HttpGet]
        public async Task<IEnumerable<string>> GetStanowisko()
            => await _context.Kontakty.OrderBy(k => k.Stanowisko).Select(k => k.Stanowisko).Distinct().ToListAsync();

        [HttpGet]
        public async Task<IEnumerable<string>> GetPion()
            => await _context.Kontakty.OrderBy(k => k.Pion).Select(k => k.Pion).Distinct().ToListAsync();



        //******************** funkcje google maps *********************
        [HttpPost]
        public async Task<IActionResult> AddAddress(string linia)
        {

            //var line = System.Net.WebUtility.UrlDecode(Request.QueryString.Value);
       
            System.IO.File.AppendAllLines("c:\\tmp\\adresy_geo_maps.csv", new List<string>() { linia });

            return Ok(true);
        }


        [HttpPost]
        public async Task<IEnumerable<KontaktyJednostkiKas>> GetJednostkiKas()
        {
            var result = await _context.KontaktyJednostkiKas.ToListAsync();
            return result;
        }
        

        //**************************************************************
    }
}