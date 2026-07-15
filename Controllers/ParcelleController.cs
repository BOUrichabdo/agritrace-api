

using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParcelleController : ControllerBase
{
    private readonly AppDbContext _context;

    public ParcelleController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/parcelle?societeId={societeId}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParcelleDto>>> GetParcelles([FromQuery] int societeId)
    {
        if (societeId <= 0)
            return BadRequest("SocieteId invalide.");

        var parcelles = await _context.Parcelles
            .Include(p => p.Ferme)
            .ThenInclude(f => f.Agriculteur)
            .Where(p => p.SocieteId == societeId)


            .Select(p => new ParcelleDto
            {
                Id = p.Id,
                NomParcelle = p.NomParcelle,
                FermeId = p.FermeId,
                NomFerme = p.Ferme.NomFerme
            })
            .ToListAsync();

        return Ok(parcelles);
    }

    // GET: api/parcelle/5?societeId={societeId}
    [HttpGet("{id}")]
    public async Task<ActionResult<ParcelleDto>> GetParcelle(int id, [FromQuery] int societeId)
    {
        if (societeId <= 0)
            return BadRequest("SocieteId invalide.");

        var parcelle = await _context.Parcelles
            .Include(p => p.Ferme)
            .ThenInclude(f => f.Agriculteur)
            .Where(p => p.Id == id && p.SocieteId == societeId)
            .Select(p => new ParcelleDto
            {
                Id = p.Id,
                NomParcelle = p.NomParcelle,
                FermeId = p.FermeId,
                NomFerme = p.Ferme.NomFerme
            })
            .FirstOrDefaultAsync();

        if (parcelle == null)
            return NotFound("Parcelle non trouvée ou non autorisée.");

        return Ok(parcelle);
    }

    // POST: api/parcelle?societeId={societeId}
    [HttpPost]
    public async Task<ActionResult<Parcelle>> PostParcelle(ParcelleDto dto, [FromQuery] int societeId)
    {
        if (societeId <= 0)
            return BadRequest("SocieteId invalide.");

        // Vérifier que la ferme existe et appartient à la société
        var ferme = await _context.Fermes
            .Include(f => f.Agriculteur)
            .FirstOrDefaultAsync(f => f.Id == dto.FermeId && f.Agriculteur.SocieteId == societeId);

        if (ferme == null)
            return BadRequest("Ferme invalide ou non autorisée.");

        var parcelle = new Parcelle
        {
            NomParcelle = dto.NomParcelle,
            FermeId = dto.FermeId,
            SocieteId = societeId  // Optionnel : peut être déduit via la ferme, mais on peut le stocker directement
        };

        _context.Parcelles.Add(parcelle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetParcelle), new { id = parcelle.Id, societeId }, parcelle);
    }

    // PUT: api/parcelle/5?societeId={societeId}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutParcelle(int id, ParcelleDto dto, [FromQuery] int societeId)
    {
        if (societeId <= 0)
            return BadRequest("SocieteId invalide.");

        if (id != dto.Id)
            return BadRequest("L'ID de l'URL ne correspond pas à celui du DTO.");

        // Vérifier que la parcelle existe et appartient à la société
        var existing = await _context.Parcelles
            .Include(p => p.Ferme)
            .ThenInclude(f => f.Agriculteur)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ferme.Agriculteur.SocieteId == societeId);

        if (existing == null)
            return NotFound("Parcelle non trouvée ou non autorisée.");

        // Vérifier que la nouvelle ferme (si changement) appartient à la société
        if (existing.FermeId != dto.FermeId)
        {
            var ferme = await _context.Fermes
                .Include(f => f.Agriculteur)
                .FirstOrDefaultAsync(f => f.Id == dto.FermeId && f.Agriculteur.SocieteId == societeId);
            if (ferme == null)
                return BadRequest("La ferme cible n'existe pas ou n'appartient pas à cette société.");
        }

        existing.NomParcelle = dto.NomParcelle;
        existing.FermeId = dto.FermeId;
        // existing.SocieteId reste inchangé

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ParcelleExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/parcelle/5?societeId={societeId}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParcelle(int id, [FromQuery] int societeId)
    {
        if (societeId <= 0)
            return BadRequest("SocieteId invalide.");

        var parcelle = await _context.Parcelles
            .Include(p => p.Ferme)
            .ThenInclude(f => f.Agriculteur)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ferme.Agriculteur.SocieteId == societeId);

        if (parcelle == null)
            return NotFound("Parcelle non trouvée ou non autorisée.");

        _context.Parcelles.Remove(parcelle);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/parcelle/byferme/{fermeId}?societeId={societeId}
    [HttpGet("byferme/{fermeId}")]
    public async Task<ActionResult<IEnumerable<ParcelleDto>>> GetParcellesByFerme(int fermeId, [FromQuery] int societeId)
    {
        if (societeId <= 0)
            return BadRequest("SocieteId invalide.");

        var parcelles = await _context.Parcelles
            .Include(p => p.Ferme)
            .ThenInclude(f => f.Agriculteur)
            .Where(p => p.FermeId == fermeId && p.Ferme.Agriculteur.SocieteId == societeId)
            .Select(p => new ParcelleDto
            {
                Id = p.Id,
                NomParcelle = p.NomParcelle,
                FermeId = p.FermeId,
                NomFerme = p.Ferme.NomFerme
            })
            .ToListAsync();

        return Ok(parcelles);
    }

    private bool ParcelleExists(int id)
    {
        return _context.Parcelles.Any(e => e.Id == id);
    }
}



























//using AgriTraceAPI.Data;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TracAgriApi.DTOs;
//using TracAgriApi.Models;

//namespace TracAgriApi.Controllers;  



//[Route("api/[controller]")]
//[ApiController]
//public class ParcelleController : ControllerBase
//{

//    private readonly AppDbContext ? _context;

//    public ParcelleController(AppDbContext context)
//    {
//        _context  = context;
//    }

//    // GET
//    [HttpGet]
//    public async Task<ActionResult<List<Parcelle>>> Get()
//    {
//        return await _context.Parcelles
//            .Include(p => p.Ferme)
//            .ToListAsync();
//    }

//    // recuprere les parcelle par ID societe
//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<PracelleDto>>> GetAgriculteurs([FromQuery] int societeId)
//    {
//        if (societeId <= 0)
//            return BadRequest("SocieteId invalide.");

//        var parcelles = await _context.Parcelles
//            .Where(a => a.SocieteId == societeId)
//            .Select(a => new PracelleDto
//            {
//                Id = a.Id,
//                NomParcelle = a.NomParcelle,
//                FermeId = a.FermeId,
//                SocieteId = a.SocieteId,

//            })
//            .ToListAsync();

//        return Ok(parcelles);
//    }
//    // POST
//    [HttpPost]
//    public async Task<IActionResult> Add(Parcelle p)
//    {
//        if (string.IsNullOrWhiteSpace(p.NomParcelle))
//            return BadRequest("Nom obligatoire");

//        _context.Parcelles.Add(p);

//        await _context.SaveChangesAsync();

//        return Ok(p);
//    }

//    // DELETE
//    [HttpDelete("{id}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        var p = await _context.Parcelles.FindAsync(id);

//        if (p == null)
//            return NotFound();

//        _context.Parcelles.Remove(p);

//        await _context.SaveChangesAsync();

//        return Ok();
//    }

//    // PUT
//    [HttpPut("{id}")]
//    public async Task<IActionResult> Update(int id, Parcelle p)
//    {
//        var existing =
//            await _context.Parcelles.FindAsync(id);

//        if (existing == null)
//            return NotFound();

//        existing.NomParcelle = p.NomParcelle;
//        existing.FermeId = p.FermeId;

//        await _context.SaveChangesAsync();

//        return Ok(existing);
//    }

//    // get parcelle par id fereme 

//    [HttpGet("byparcelle/{feremeid}")]
//    public async Task<ActionResult<IEnumerable<Parcelle>>> GetParcellesByFerme(int feremeid)
//    {
//        var data = await _context.Parcelles
//            .Where(f => f.FermeId == feremeid)
//            .Select(f => new Parcelle
//            {
//                Id = f.Id,
//                NomParcelle = f.NomParcelle,
//                FermeId = f.FermeId
//            })
//            .ToListAsync();

//        return Ok(data);
//    }





//}

