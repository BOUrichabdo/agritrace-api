using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace AgriTraceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgriculteurController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AgriculteurController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/agriculteur    get liste agriculteur
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            try
            {


            
            var data = await _context.Agriculteurs.ToListAsync();
            return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        // GET by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _context.Agriculteurs.FindAsync(id);
            if (a == null) return NotFound();

            return Ok(a);
        }
        // POST ajouter un agriculteur
        [HttpPost]
        public async Task<IActionResult> Add(Agriculteur a)
        {
            _context.Agriculteurs.Add(a);
            await _context.SaveChangesAsync();

            return Ok(a);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _context.Agriculteurs.FindAsync(id);
            if (a == null) return NotFound();

            _context.Agriculteurs.Remove(a);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT Modification d'un agriculteur

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Agriculteur a)
        {
            var existing = await _context.Agriculteurs.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.Nom = a.Nom;
            existing.Telephone = a.Telephone;
            existing.Adresse = a.Adresse;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }


        [HttpGet("byAgriculteur/{agriculteurId}")]
        public async Task<ActionResult<IEnumerable<FermeDto>>> GetByAgriculteur(int agriculteurId)
        {
            var data = await _context.Fermes
                .Where(f => f.AgriculteurId == agriculteurId)
                .Select(f => new FermeDto
                {
                    Id = f.Id,
                    NomFerme = f.NomFerme,
                    AgriculteurId = f.AgriculteurId
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}