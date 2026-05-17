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
    public class CategoriesController : ControllerBase
    {

        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // 📌 GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll()
        {
            var data = await _context.Categories
                .Select(c => new CategorieDto
                {
                    Id = c.Id,
                    Intitule = c.Intitule
                })
                .ToListAsync();

            return Ok(data);
        }

        // 📌 GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategorieDto>> GetById(int id)
        {
            var c = await _context.Categories.FindAsync(id);

            if (c == null)
                return NotFound();

            return Ok(new CategorieDto
            {
                Id = c.Id,
                Intitule = c.Intitule
            });
        }

        // 📌 POST: api/Categories
        [HttpPost]
        public async Task<ActionResult> Create(CategorieDto dto)
        {
            var categorie = new Categorie
            {
                Intitule = dto.Intitule
            };

            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();

            return Ok(categorie);
        }

        // 📌 PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategorieDto dto)
        {
            var categorie = await _context.Categories.FindAsync(id);

            if (categorie == null)
                return NotFound();

            categorie.Intitule = dto.Intitule;

            await _context.SaveChangesAsync();

            return Ok(categorie);
        }

        // 📌 DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);

            if (categorie == null)
                return NotFound();

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
