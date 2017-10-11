//using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IASServices.Models;



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
            return await _context.Upowaznienia.ToListAsync();
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

        // POST: api/Kontakties
        [HttpPost]
        public async Task<IActionResult> PostUpowaznienia([FromBody] Upowaznienia upowaznienia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Upowaznienia.Add(upowaznienia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUpowaznienia", new { id = upowaznienia.Id }, upowaznienia);
        }


        // PUT: api/Kontakties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUpowaznienia([FromRoute] long id, [FromBody] Upowaznienia upowaznienia)
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


        //[HttpGet]
        //public async Task<IEnumerable<Upowaznienia>> GetUpowaznieniaLista()
        //{
        //    //var queryString = HttpContext.Request.Query;
        //    return await (new IASServices.Models.UpowaznieniaContext(null)).Upowaznienia.ToListAsync();
        //}



    }
}