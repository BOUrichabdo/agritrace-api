using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FermeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FermeController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<ActionResult<List<FermeDto>>> Get()
        {
            return await _context.Fermes
                .Include(f => f.Agriculteur)
                .Select(f => new FermeDto
                {
                    Id = f.Id,
                    NomFerme = f.NomFerme,
                    AgriculteurId = f.AgriculteurId,
                    NomAgriculteur = f.Agriculteur.Nom
                })
                .ToListAsync();
        }
        // je vais modifier le get all pour qu'il prenne en compte le societeId

        [HttpGet("societe/{societeId}")]
        public async Task<ActionResult<List<FermeDto>>> GetBySocieteId(int societeId)
        {
            var fermes = await _context.Fermes
                .Include(f => f.Agriculteur)
                .Where(f => f.Agriculteur.SocieteId == societeId)
                .Select(f => new FermeDto
                {
                    Id = f.Id,
                    NomFerme = f.NomFerme,
                    AgriculteurId = f.AgriculteurId,
                    NomAgriculteur = f.Agriculteur.Nom
                })
                .ToListAsync();
            return Ok(fermes);
        }









        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ferme = await _context.Fermes
                .Include(f => f.Agriculteur)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (ferme == null)
                return NotFound();

            return Ok(ferme);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Add(Ferme a)
        {
            if (string.IsNullOrEmpty(a.NomFerme))
                return BadRequest("Nom ferme obligatoire");
            _context.Fermes.Add(a);
            await _context.SaveChangesAsync();
            return Ok(a);   
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Ferme a)
        {
            if (id != a.Id)
                return BadRequest("ID mismatch");

            var existing = await _context.Fermes.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.NomFerme = a.NomFerme;
            existing.AgriculteurId = a.AgriculteurId; // 🔥 IMPORTANT

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.Fermes.FindAsync(id);

            if (existing == null)
                return NotFound();

            _context.Fermes.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok();
        }

      




    }
}
