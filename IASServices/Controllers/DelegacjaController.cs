using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncWebIAS.Model;
using IASServices.Repositories;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Web.OData;

namespace IASServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DelegacjaController : Controller
    {
        private readonly IData<Delegacja, long> _repo;

        public DelegacjaController(IData<Delegacja, long> repo)
        {
            _repo = repo;
        }

        // GET: api/Delegacjas       
        [HttpGet]
        //[EnableQuery]
        public IEnumerable<Delegacja> GetDelegacja()//(string inlinecount, int skip, int top)
        {
            //var queryString = HttpContext.Request.QueryString;
            var query = HttpContext.Request.Query;
            //var take = Convert.ToInt32(query["take"]);
            //var skip = Convert.ToInt32(query["skip"]);
            //var qd = queryString.Value;
            //int skip = Convert.ToInt32(1);// (queryString["$skip"]);
            //int take = Convert.ToInt32(2);// queryString["$top"]);
            //var list = _repo.GetAll().Skip(skip).Take(take).ToList();
            //return new
            //{
            //    Items = data.Skip(skip).Take(take),
            //    Count = data.Count()
            //};

            //var t = skip;
            //var list = _repo.GetAll().Take(100);
            //return Json(new { result = list, count = list.Count() });
            return _repo.GetAll();//.Take(40);
            //return _repo.GetAll().Take(take).Skip(skip);
        }

        // GET: api/Delegacjas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDelegacja([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var delegacja = _repo.Get(id);

            if (delegacja == null)
            {
                return NotFound();
            }

            //return Json(new { result = delegacja, count = 1 });
            return Ok(delegacja);
        }

        // PUT: api/Delegacjas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDelegacja([FromRoute] long id, [FromBody] Delegacja delegacja)
        {
            var t = HttpContext.Request;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != delegacja.Id)
            {
                return BadRequest();
            }

            int res = _repo.Update(id, delegacja);
            if (res != 0) return Ok(res);

            //_context.Entry(delegacja).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!DelegacjaExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return NoContent();
        }

        // POST: api/Delegacjas
        [HttpPost]
        public async Task<IActionResult> PostDelegacja([FromBody] Delegacja delegacja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int res = _repo.Add(delegacja);
            if (res != 0) return Ok();
            return Forbid();

            //_context.Delegacja.Add(delegacja);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetDelegacja", new { id = delegacja.Id }, delegacja);
        }

        // DELETE: api/Delegacjas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelegacja([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int res = _repo.Delete(id);
            if (res != 0) return Ok();
            return NotFound();

            //var delegacja = await _context.Delegacja.SingleOrDefaultAsync(m => m.Id == id);
            //if (delegacja == null)
            //{
            //    return NotFound();
            //}

            //_context.Delegacja.Remove(delegacja);
            //await _context.SaveChangesAsync();

            //return Ok(delegacja);
        }

        //private bool DelegacjaExists(long id)
        //{
        //    return _context.Delegacja.Any(e => e.Id == id);
        //}
    }
}