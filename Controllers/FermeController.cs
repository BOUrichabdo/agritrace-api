

using AgriTraceAPI.Data;
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

        // GET: api/ferme?societeId={societeId}


        [HttpGet]
        public async Task<ActionResult<IEnumerable<FermeDto>>> GetFermes([FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

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



        // GET: api/ferme/5?societeId={societeId}
        [HttpGet("{id}")]
        public async Task<ActionResult<FermeDto>> GetFerme(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var ferme = await _context.Fermes
                .Include(f => f.Agriculteur)
                .Where(f => f.Id == id && f.Agriculteur.SocieteId == societeId)
                .Select(f => new FermeDto
                {
                    Id = f.Id,
                    NomFerme = f.NomFerme,
                    AgriculteurId = f.AgriculteurId,
                    NomAgriculteur = f.Agriculteur.Nom
                })
                .FirstOrDefaultAsync();

            if (ferme == null)
                return NotFound("Ferme non trouvée ou non autorisée.");

            return Ok(ferme);
        }

        // POST: api/ferme?societeId={societeId}
        [HttpPost]
        public async Task<ActionResult<Ferme>> PostFerme(FermeDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            // Vérifier que l'agriculteur existe et appartient à la société
            var agriculteur = await _context.Agriculteurs
                .FirstOrDefaultAsync(a => a.Id == dto.AgriculteurId && a.SocieteId == societeId);

            if (agriculteur == null)
                return BadRequest("Agriculteur invalide ou non autorisé.");

            var ferme = new Ferme
            {
                NomFerme = dto.NomFerme,
                AgriculteurId = dto.AgriculteurId,
                SocieteId = societeId
                // SocieteId est déduit via l'agriculteur
            };

            _context.Fermes.Add(ferme);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFerme), new { id = ferme.Id, societeId }, ferme);
        }
        // PUT: api/ferme/5?societeId={societeId}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFerme(int id, FermeDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            if (id != dto.Id)
                return BadRequest("L'ID de l'URL ne correspond pas à celui du DTO.");

            // Vérifier que la ferme existe et appartient à la société
            var existing = await _context.Fermes
                .Include(f => f.Agriculteur)
                .FirstOrDefaultAsync(f => f.Id == id && f.Agriculteur.SocieteId == societeId);

            if (existing == null)
                return NotFound("Ferme non trouvée ou non autorisée.");

            // Vérifier que le nouvel agriculteur (si changé) appartient à la société
            if (existing.AgriculteurId != dto.AgriculteurId)
            {
                var newAgriculteur = await _context.Agriculteurs
                    .FirstOrDefaultAsync(a => a.Id == dto.AgriculteurId && a.SocieteId == societeId);
                if (newAgriculteur == null)
                    return BadRequest("L'agriculteur cible n'existe pas ou n'appartient pas à cette société.");
            }

            existing.NomFerme = dto.NomFerme;
            existing.AgriculteurId = dto.AgriculteurId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FermeExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }




        // DELETE: api/ferme/5?societeId={societeId}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFerme(int id, [FromQuery] int societeId)
        //{
        //    if (societeId <= 0)
        //        return BadRequest("SocieteId invalide.");
        //    var ferme = await _context.Fermes
        //        .Include(f => f.Agriculteur)
        //        .FirstOrDefaultAsync(f => f.Id == id && f.Agriculteur.SocieteId == societeId);

        //    if (ferme == null)
        //        return NotFound("Ferme non trouvée ou non autorisée.");

        //    _context.Fermes.Remove(ferme);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFerme(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            // Récupérer la ferme ET vérifier si elle a des parcelles en une seule requête
            var fermeData = await _context.Fermes
                .Include(f => f.Agriculteur)
                .Where(f => f.Id == id && f.Agriculteur.SocieteId == societeId)
                .Select(f => new
                {
                    Ferme = f,
                    HasParcelles = _context.Parcelles.Any(p => p.FermeId == f.Id)
                })
                .FirstOrDefaultAsync();

            if (fermeData == null)
                return NotFound("Ferme non trouvée ou non autorisée.");

            if (fermeData.HasParcelles)
                return Conflict("Impossible de supprimer cette ferme car elle possède des parcelles associées. Supprimez d'abord les parcelles.");

            // Supprimer la ferme
            _context.Fermes.Remove(fermeData.Ferme);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FermeExists(int id)
        {
            return _context.Fermes.Any(e => e.Id == id);
        }

        // GET: api/Ferme/byAgriculteur/{agriculteurId}?societeId={societeId}
        [HttpGet("byAgriculteur/{agriculteurId}")]
        public async Task<ActionResult<IEnumerable<FermeDto>>> GetByAgriculteur(
            int agriculteurId,
            [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            // Vérifier que l'agriculteur existe et appartient à la société
            var agriculteur = await _context.Agriculteurs
                .FirstOrDefaultAsync(a => a.Id == agriculteurId && a.SocieteId == societeId);
            if (agriculteur == null)
                return NotFound("Agriculteur non trouvé ou non autorisé.");

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















//using AgriTraceAPI.Data;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TracAgriApi.DTOs;
//using TracAgriApi.Models;

//namespace TracAgriApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class FermeController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public FermeController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET ALL
//        [HttpGet]
//        public async Task<ActionResult<List<FermeDto>>> Get()
//        {
//            return await _context.Fermes
//                .Include(f => f.Agriculteur)
//                .Select(f => new FermeDto
//                {
//                    Id = f.Id,
//                    NomFerme = f.NomFerme,
//                    AgriculteurId = f.AgriculteurId,
//                    NomAgriculteur = f.Agriculteur.Nom
//                })
//                .ToListAsync();
//        }
//        // GET BY ID
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var ferme = await _context.Fermes
//                .Include(f => f.Agriculteur)
//                .FirstOrDefaultAsync(f => f.Id == id);

//            if (ferme == null)
//                return NotFound();

//            return Ok(ferme);
//        }

//        // POST
//        [HttpPost]
//        public async Task<IActionResult> Add(Ferme a)
//        {
//            if (string.IsNullOrEmpty(a.NomFerme))
//                return BadRequest("Nom ferme obligatoire");
//            _context.Fermes.Add(a);
//            await _context.SaveChangesAsync();
//            return Ok(a);   
//        }

//        // PUT
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, Ferme a)
//        {
//            if (id != a.Id)
//                return BadRequest("ID mismatch");

//            var existing = await _context.Fermes.FindAsync(id);

//            if (existing == null)
//                return NotFound();

//            existing.NomFerme = a.NomFerme;
//            existing.AgriculteurId = a.AgriculteurId; // 🔥 IMPORTANT

//            await _context.SaveChangesAsync();

//            return Ok(existing);
//        }

//        // DELETE
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var existing = await _context.Fermes.FindAsync(id);

//            if (existing == null)
//                return NotFound();

//            _context.Fermes.Remove(existing);
//            await _context.SaveChangesAsync();

//            return Ok();
//        }


//        // adapter ce 





//    }
//}
