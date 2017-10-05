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

        [HttpGet]
        public async Task<IEnumerable<Kontakty>> GetKontakty()
        {
            //var queryString = HttpContext.Request.Query;
            return await (new IASServices.Models.KontaktyContext(null)).Kontakty.ToListAsync();
        }

        // GET: api/Kontakties/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKontakty([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kontakty = await (new IASServices.Models.KontaktyContext(null)).Kontakty.SingleOrDefaultAsync(m => m.Id == id);

            if (kontakty == null)
            {
                return NotFound();
            }

            return Ok(kontakty);
        }

    }
}