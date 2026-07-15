using AgriTraceAPI.Data;
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

        // GET: api/Categories?societeId={societeId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll([FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var data = await _context.Categories
                .Where(c => c.SocieteId == societeId)
                .Select(c => new CategorieDto
                {
                    Id = c.Id,
                    Intitule = c.Intitule
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/Categories/5?societeId={societeId}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategorieDto>> GetById(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var c = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.SocieteId == societeId);

            if (c == null)
                return NotFound("Catégorie non trouvée ou non autorisée.");

            return Ok(new CategorieDto
            {
                Id = c.Id,
                Intitule = c.Intitule
            });
        }

        // POST: api/Categories?societeId={societeId}
        [HttpPost]
        public async Task<ActionResult> Create(CategorieDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var categorie = new Categorie
            {
                Intitule = dto.Intitule,
                SocieteId = societeId
            };

            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = categorie.Id, societeId }, categorie);
        }

        // PUT: api/Categories/5?societeId={societeId}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategorieDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var categorie = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.SocieteId == societeId);

            if (categorie == null)
                return NotFound("Catégorie non trouvée ou non autorisée.");

            categorie.Intitule = dto.Intitule;
            // SocieteId ne change pas

            await _context.SaveChangesAsync();

            return Ok(categorie);
        }

        // DELETE: api/Categories/5?societeId={societeId}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var categorie = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.SocieteId == societeId);

            if (categorie == null)
                return NotFound("Catégorie non trouvée ou non autorisée.");

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();

            return NoContent(); // ou Ok()
        }
    }
}



































//using AgriTraceAPI.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TracAgriApi.DTOs;
//using TracAgriApi.Models;

//namespace TracAgriApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CategoriesController : ControllerBase
//    {

//        private readonly AppDbContext _context;

//        public CategoriesController(AppDbContext context)
//        {
//            _context = context;
//        }


//        // 📌 GET: api/Categories
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll()
//        {
//            var data = await _context.Categories
//                .Select(c => new CategorieDto
//                {
//                    Id = c.Id,
//                    Intitule = c.Intitule
//                })
//                .ToListAsync();

//            return Ok(data);
//        }

//        // 📌 GET: api/Categories/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<CategorieDto>> GetById(int id)
//        {
//            var c = await _context.Categories.FindAsync(id);

//            if (c == null)
//                return NotFound();

//            return Ok(new CategorieDto
//            {
//                Id = c.Id,
//                Intitule = c.Intitule
//            });
//        }

//        // 📌 POST: api/Categories
//        [HttpPost]
//        public async Task<ActionResult> Create(CategorieDto dto)
//        {
//            var categorie = new Categorie
//            {
//                Intitule = dto.Intitule
//            };

//            _context.Categories.Add(categorie);
//            await _context.SaveChangesAsync();

//            return Ok(categorie);
//        }

//        // 📌 PUT: api/Categories/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, CategorieDto dto)
//        {
//            var categorie = await _context.Categories.FindAsync(id);

//            if (categorie == null)
//                return NotFound();

//            categorie.Intitule = dto.Intitule;

//            await _context.SaveChangesAsync();

//            return Ok(categorie);
//        }

//        // 📌 DELETE: api/Categories/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var categorie = await _context.Categories.FindAsync(id);

//            if (categorie == null)
//                return NotFound();

//            _context.Categories.Remove(categorie);
//            await _context.SaveChangesAsync();

//            return Ok();
//        }
//    }
//}











