using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VarietesController : ControllerBase
    {



        private readonly AppDbContext _context;

        public VarietesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Varietes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VarieteDto>>> GetAll()
        {
            var data = await _context.Varietes
                .Include(v => v.Categorie)
                .Select(v => new VarieteDto
                {
                    Id = v.Id,
                    Intitule = v.Intitule,
                    CategorieId = v.CategorieId,
                    CategorieNom = v.Categorie != null ? v.Categorie.Intitule : ""
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/Varietes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VarieteDto>> GetById(int id)
        {
            var v = await _context.Varietes
                .Include(x => x.Categorie)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (v == null)
                return NotFound();

            return Ok(new VarieteDto
            {
                Id = v.Id,
                Intitule = v.Intitule,
                CategorieId = v.CategorieId,
                CategorieNom = v.Categorie?.Intitule
            });
        }

        // POST: api/Varietes
        [HttpPost]
        public async Task<ActionResult> Create(VarieteDto dto)
        {
            var variete = new Variete
            {
                Intitule = dto.Intitule,
                CategorieId = dto.CategorieId
            };

            _context.Varietes.Add(variete);

            await _context.SaveChangesAsync();

            return Ok(new VarieteDto
            {
                Id = variete.Id,
                Intitule = variete.Intitule,
                CategorieId = variete.CategorieId
            });
        }

        // PUT: api/Varietes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, VarieteDto dto)
        {
            var variete = await _context.Varietes
                .Include(v => v.Categorie)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (variete == null)
                return NotFound();

            // 🔥 modification
            variete.Intitule = dto.Intitule;
            variete.CategorieId = dto.CategorieId;

            await _context.SaveChangesAsync();

            // 🔥 recharger catégorie après update
            await _context.Entry(variete)
                .Reference(v => v.Categorie)
                .LoadAsync();

            // 🔥 retourner DTO
            var result = new VarieteDto
            {
                Id = variete.Id,
                Intitule = variete.Intitule,
                CategorieId = variete.CategorieId,
                CategorieNom = variete.Categorie?.Intitule
            };

            return Ok(result);
        }

        // DELETE: api/Varietes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var variete = await _context.Varietes.FindAsync(id);

            if (variete == null)
                return NotFound();

            _context.Varietes.Remove(variete);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
