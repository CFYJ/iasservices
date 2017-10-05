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
        public async Task<IEnumerable<Kontakty>> GetKontakty()
        {
            //var queryString = HttpContext.Request.Query;
            return await _context.Kontakty.ToListAsync();
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

    }
}