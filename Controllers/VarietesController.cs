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

        // GET: api/Varietes?societeId={societeId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VarieteDto>>> GetAll([FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var data = await _context.Varietes
                .Include(v => v.Categorie)
                .Where(v => v.Categorie.SocieteId == societeId)   // Filtre par société via la catégorie
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

        // GET: api/Varietes/5?societeId={societeId}
        [HttpGet("{id}")]
        public async Task<ActionResult<VarieteDto>> GetById(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var v = await _context.Varietes
                .Include(x => x.Categorie)
                .FirstOrDefaultAsync(x => x.Id == id && x.Categorie.SocieteId == societeId);

            if (v == null)
                return NotFound("Variété non trouvée ou non autorisée.");

            return Ok(new VarieteDto
            {
                Id = v.Id,
                Intitule = v.Intitule,
                CategorieId = v.CategorieId,
                CategorieNom = v.Categorie?.Intitule
            });
        }

        // POST: api/Varietes?societeId={societeId}
        [HttpPost]
        public async Task<ActionResult> Create(VarieteDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            // Vérifier que la catégorie spécifiée appartient bien à la société
            var categorie = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategorieId && c.SocieteId == societeId);

            if (categorie == null)
                return BadRequest("La catégorie spécifiée n'existe pas ou n'appartient pas à votre société.");

            var variete = new Variete
            {
                Intitule = dto.Intitule,
                CategorieId = dto.CategorieId
            };

            _context.Varietes.Add(variete);
            await _context.SaveChangesAsync();

            // Recharger la catégorie pour avoir le nom
            await _context.Entry(variete).Reference(v => v.Categorie).LoadAsync();

            var result = new VarieteDto
            {
                Id = variete.Id,
                Intitule = variete.Intitule,
                CategorieId = variete.CategorieId,
                CategorieNom = variete.Categorie?.Intitule
            };

            return CreatedAtAction(nameof(GetById), new { id = variete.Id, societeId }, result);
        }

        // PUT: api/Varietes/5?societeId={societeId}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, VarieteDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var variete = await _context.Varietes
                .Include(v => v.Categorie)
                .FirstOrDefaultAsync(v => v.Id == id && v.Categorie.SocieteId == societeId);

            if (variete == null)
                return NotFound("Variété non trouvée ou non autorisée.");

            // Vérifier que la nouvelle catégorie appartient bien à la société
            if (variete.CategorieId != dto.CategorieId)
            {
                var categorie = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == dto.CategorieId && c.SocieteId == societeId);
                if (categorie == null)
                    return BadRequest("La catégorie spécifiée n'existe pas ou n'appartient pas à votre société.");
            }

            variete.Intitule = dto.Intitule;
            variete.CategorieId = dto.CategorieId;

            await _context.SaveChangesAsync();

            // Recharger la catégorie pour le DTO de retour
            await _context.Entry(variete).Reference(v => v.Categorie).LoadAsync();

            var result = new VarieteDto
            {
                Id = variete.Id,
                Intitule = variete.Intitule,
                CategorieId = variete.CategorieId,
                CategorieNom = variete.Categorie?.Intitule
            };

            return Ok(result);
        }

        // DELETE: api/Varietes/5?societeId={societeId}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int societeId)
        {

            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            // 1. Vérifier que la parcelle existe et appartient à la société
            var variete = await _context.Varietes
                .Include(v => v.Categorie)
                .FirstOrDefaultAsync(v => v.Id == id && v.Categorie.SocieteId == societeId);

            if (variete == null)
                return NotFound("Variété non trouvée ou non autorisée.");

                

            // 2. Vérifier les dépendances (étiquettes)
            bool hasEtiquettes = await _context.EtiquetteFermes.AnyAsync(e => e.VarieteId == id);
            if (hasEtiquettes)
                return Conflict("Impossible de supprimer cette Variété car elle possède des étiquettes associées. Supprimez d'abord les étiquettes.");






            // 3. Vérifier les dépendances (produits)
            bool hasProduits = await _context.Produites.AnyAsync(p => p.VarieteId == id);
            if (hasProduits)
                return Conflict("Impossible de supprimer cette Variété car elle possède des produits associés. Supprimez d'abord les produits.");

            // verification

            qsf



            // 4. Supprimer la parcelle
            _context.Varietes.Remove(variete);
            await _context.SaveChangesAsync();

            return NoContent();









            //if (societeId <= 0)
            //    return BadRequest("SocieteId invalide.");

            //var variete = await _context.Varietes
            //    .Include(v => v.Categorie)
            //    .FirstOrDefaultAsync(v => v.Id == id && v.Categorie.SocieteId == societeId);

            //if (variete == null)
            //    return NotFound("Variété non trouvée ou non autorisée.");

            //_context.Varietes.Remove(variete);
            //await _context.SaveChangesAsync();

            //return NoContent();
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
//    public class VarietesController : ControllerBase
//    {



//        private readonly AppDbContext _context;

//        public VarietesController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Varietes
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<VarieteDto>>> GetAll()
//        {
//            var data = await _context.Varietes
//                .Include(v => v.Categorie)
//                .Select(v => new VarieteDto
//                {
//                    Id = v.Id,
//                    Intitule = v.Intitule,
//                    CategorieId = v.CategorieId,
//                    CategorieNom = v.Categorie != null ? v.Categorie.Intitule : ""
//                })
//                .ToListAsync();

//            return Ok(data);
//        }

//        // GET: api/Varietes/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<VarieteDto>> GetById(int id)
//        {
//            var v = await _context.Varietes
//                .Include(x => x.Categorie)
//                .FirstOrDefaultAsync(x => x.Id == id);

//            if (v == null)
//                return NotFound();

//            return Ok(new VarieteDto
//            {
//                Id = v.Id,
//                Intitule = v.Intitule,
//                CategorieId = v.CategorieId,
//                CategorieNom = v.Categorie?.Intitule
//            });
//        }

//        // POST: api/Varietes
//        [HttpPost]
//        public async Task<ActionResult> Create(VarieteDto dto)
//        {
//            var variete = new Variete
//            {
//                Intitule = dto.Intitule,
//                CategorieId = dto.CategorieId
//            };

//            _context.Varietes.Add(variete);

//            await _context.SaveChangesAsync();

//            return Ok(new VarieteDto
//            {
//                Id = variete.Id,
//                Intitule = variete.Intitule,
//                CategorieId = variete.CategorieId
//            });
//        }

//        // PUT: api/Varietes/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, VarieteDto dto)
//        {
//            var variete = await _context.Varietes
//                .Include(v => v.Categorie)
//                .FirstOrDefaultAsync(v => v.Id == id);

//            if (variete == null)
//                return NotFound();

//            // 🔥 modification
//            variete.Intitule = dto.Intitule;
//            variete.CategorieId = dto.CategorieId;

//            await _context.SaveChangesAsync();

//            // 🔥 recharger catégorie après update
//            await _context.Entry(variete)
//                .Reference(v => v.Categorie)
//                .LoadAsync();

//            // 🔥 retourner DTO
//            var result = new VarieteDto
//            {
//                Id = variete.Id,
//                Intitule = variete.Intitule,
//                CategorieId = variete.CategorieId,
//                CategorieNom = variete.Categorie?.Intitule
//            };

//            return Ok(result);
//        }

//        // DELETE: api/Varietes/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var variete = await _context.Varietes.FindAsync(id);

//            if (variete == null)
//                return NotFound();

//            _context.Varietes.Remove(variete);
//            await _context.SaveChangesAsync();

//            return Ok();
//        }
//    }
//}
