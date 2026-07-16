
using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgriculteurController : ControllerBase
    {
        // db context  BD 
        private readonly AppDbContext _context;

        public AgriculteurController(AppDbContext context)
        {
            _context = context;
        }

        // recuprere les agriculteur par ID Societe  ALL Agri where ID societe
        // GET: api/agriculteur?societeId={societeId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgriculteurDto>>> GetAgriculteurs([FromQuery] int societeId)
        {
            // verifier Id societe
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");
            // recuprere agriculteur avec ces info en base sur DTO agriculteur 
            var agriculteurs = await _context.Agriculteurs
                .Where(a => a.SocieteId == societeId)
                .Select(a => new AgriculteurDto
                {
                    Id = a.Id,
                    Nom = a.Nom,
                    Adresse = a.Adresse,
                    Telephone = a.Telephone,
                })
                .ToListAsync();

            return Ok(agriculteurs);
        }



        // recuprere les agri par ID et ID societe 
        // GET: api/agriculteur/5?societeId={societeId}
        [HttpGet("{id}")]
        public async Task<ActionResult<AgriculteurDto>> GetAgriculteur(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var agriculteur = await _context.Agriculteurs
                .Where(a => a.Id == id && a.SocieteId == societeId)
                .Select(a => new AgriculteurDto
                {
                    Id = a.Id,
                    Nom = a.Nom,
                    Adresse = a.Adresse,
                    Telephone = a.Telephone
                })
                .FirstOrDefaultAsync();

            if (agriculteur == null)
                return NotFound("Agriculteur non trouvé ou non autorisé.");

            return Ok(agriculteur);
        }





        // La creation de agriculteur 
        // POST: api/agriculteur?societeId={societeId}
        [HttpPost]
        public async Task<ActionResult<Agriculteur>> PostAgriculteur(AgriculteurDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var agriculteur = new Agriculteur
            {
                Nom = dto.Nom,
                Adresse = dto.Adresse,
                Telephone = dto.Telephone,
                SocieteId = societeId
            };

            _context.Agriculteurs.Add(agriculteur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAgriculteur), new { id = agriculteur.Id, societeId }, agriculteur);
        }




        // Modifier agriculteur par ID agriculteur et ID Societe 
        // PUT: api/agriculteur/5?societeId={societeId}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgriculteur(int id, AgriculteurDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            if (id != dto.Id)
                return BadRequest("L'ID de l'URL ne correspond pas à celui du DTO.");

            // Vérifier que l'agriculteur appartient bien à la société
            var existing = await _context.Agriculteurs
                .FirstOrDefaultAsync(a => a.Id == id && a.SocieteId == societeId);

            if (existing == null)
                return NotFound("Agriculteur non trouvé ou non autorisé.");

            // Mise à jour des champs
            existing.Nom = dto.Nom;
            existing.Adresse = dto.Adresse;
            existing.Telephone = dto.Telephone;
            // SocieteId ne change pas

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgriculteurExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }




        // supp Agri par ID et Id societe 
        // DELETE: api/agriculteur/5?societeId={societeId}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgriculteur(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var agriculteur = await _context.Agriculteurs
                .FirstOrDefaultAsync(a => a.Id == id && a.SocieteId == societeId);

            if (agriculteur == null)
                return NotFound("Agriculteur non trouvé ou non autorisé.");

            _context.Agriculteurs.Remove(agriculteur);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // verifier si agriculteur existe par ID
        private bool AgriculteurExists(int id)
        {
            return _context.Agriculteurs.Any(e => e.Id == id);
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
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TracAgriApi.DTOs;
//using TracAgriApi.Models;

//namespace AgriTraceAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AgriculteurController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public AgriculteurController(AppDbContext context)
//        {
//            _context = context;
//        }
//        // GET: api/agriculteur    get liste agriculteur
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {

//            try
//            {



//            var data = await _context.Agriculteurs.ToListAsync();
//            return Ok(data);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    error = ex.Message,
//                    innerError = ex.InnerException?.Message,
//                    stackTrace = ex.StackTrace
//                });
//            }
//        }
//        // GET by id
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var a = await _context.Agriculteurs.FindAsync(id);
//            if (a == null) return NotFound();

//            return Ok(a);
//        }
//        // POST ajouter un agriculteur
//        [HttpPost]
//        public async Task<IActionResult> Add(Agriculteur a)
//        {
//            _context.Agriculteurs.Add(a);
//            await _context.SaveChangesAsync();

//            return Ok(a);
//        }

//        // DELETE
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var a = await _context.Agriculteurs.FindAsync(id);
//            if (a == null) return NotFound();

//            _context.Agriculteurs.Remove(a);
//            await _context.SaveChangesAsync();

//            return Ok();
//        }

//        // PUT Modification d'un agriculteur

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, Agriculteur a)
//        {
//            var existing = await _context.Agriculteurs.FindAsync(id);

//            if (existing == null)
//                return NotFound();

//            existing.Nom = a.Nom;
//            existing.Telephone = a.Telephone;
//            existing.Adresse = a.Adresse;

//            await _context.SaveChangesAsync();

//            return Ok(existing);
//        }





//        [HttpGet("byAgriculteur/{agriculteurId}")]
//        public async Task<ActionResult<IEnumerable<FermeDto>>> GetByAgriculteur(int agriculteurId)
//        {
//            var data = await _context.Fermes
//                .Where(f => f.AgriculteurId == agriculteurId)
//                .Select(f => new FermeDto
//                {
//                    Id = f.Id,
//                    NomFerme = f.NomFerme,
//                    AgriculteurId = f.AgriculteurId
//                })
//                .ToListAsync();

//            return Ok(data);
//        }
//    }
//}