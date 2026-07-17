using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduitsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProduitsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Produits?societeId={societeId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll([FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var data = await _context.Produites
                .Include(p => p.Parcelle)
                    
                .Include(p => p.Variete)
                    .ThenInclude(v => v.Categorie)
                .Where(p => p.SocieteId == societeId) 
                .Select(p => new ProduitDto
                {
                    Id = p.Id,
                    Nom = p.Nom,
                    ParcelleId = p.ParcelleId,
                    ParcelleNom = p.Parcelle != null ? p.Parcelle.NomParcelle : "",
                    VarieteId = p.VarieteId,
                    VarieteNom = p.Variete != null ? p.Variete.Intitule : "",
                    CategorieNom = p.Variete != null && p.Variete.Categorie != null
                        ? p.Variete.Categorie.Intitule
                        : ""
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/Produits/5?societeId={societeId}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProduitDto>> GetById(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var p = await _context.Produites
                .Include(x => x.Parcelle)
                   
                .Include(x => x.Variete)
                    .ThenInclude(v => v.Categorie)
                .FirstOrDefaultAsync(x => x.Id == id && x.SocieteId == societeId);

            if (p == null)
                return NotFound("Produit non trouvé ou non autorisé.");

            var dto = new ProduitDto
            {
                Id = p.Id,
                Nom = p.Nom,
                ParcelleId = p.ParcelleId,
                ParcelleNom = p.Parcelle?.NomParcelle,
                VarieteId = p.VarieteId,
                VarieteNom = p.Variete?.Intitule,
                CategorieNom = p.Variete?.Categorie?.Intitule
            };

            return Ok(dto);
        }

        // POST: api/Produits?societeId={societeId}
        [HttpPost]
        public async Task<ActionResult> Create(ProduitDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            // Vérifier que la parcelle appartient bien à la société
            var parcelle = await _context.Parcelles
                
                .FirstOrDefaultAsync(p => p.Id == dto.ParcelleId && p.SocieteId == societeId);

            if (parcelle == null)
                return BadRequest("La parcelle spécifiée n'existe pas ou n'appartient pas à votre société.");

            // Vérifier que la variété appartient bien à la société (via sa catégorie)
            var variete = await _context.Varietes
                .Include(v => v.Categorie)
                .FirstOrDefaultAsync(v => v.Id == dto.VarieteId && v.Categorie.SocieteId == societeId);

            if (variete == null)
                return BadRequest("La variété spécifiée n'existe pas ou n'appartient pas à votre société.");

            var produit = new Produit
            {
                Nom = dto.Nom,
                ParcelleId = dto.ParcelleId,
                VarieteId = dto.VarieteId,
                SocieteId = societeId
            };

            _context.Produites.Add(produit);
            await _context.SaveChangesAsync();

            // Recharger les relations pour le DTO de retour
            await _context.Entry(produit).Reference(p => p.Parcelle).LoadAsync();
            //await _context.Entry(produit).Reference(p => p.Variete).ThenInclude(v => v.Categorie).LoadAsync();

            var result = new ProduitDto
            {
                Id = produit.Id,
                Nom = produit.Nom,
                ParcelleId = produit.ParcelleId,
                ParcelleNom = produit.Parcelle?.NomParcelle,
                VarieteId = produit.VarieteId,
                VarieteNom = produit.Variete?.Intitule,
                CategorieNom = produit.Variete?.Categorie?.Intitule
            };

            return CreatedAtAction(nameof(GetById), new { id = produit.Id, societeId }, result);
        }

        // PUT: api/Produits/5?societeId={societeId}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProduitDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var produit = await _context.Produites
                .Include(p => p.Parcelle)
                    
                .FirstOrDefaultAsync(p => p.Id == id && p.SocieteId == societeId);

            if (produit == null)
                return NotFound("Produit non trouvé ou non autorisé.");

            // Vérifier que la nouvelle parcelle (si modifiée) appartient à la société
            if (produit.ParcelleId != dto.ParcelleId)
            {
                var parcelle = await _context.Parcelles
                   
                    .FirstOrDefaultAsync(p => p.Id == dto.ParcelleId && p.SocieteId == societeId);
                if (parcelle == null)
                    return BadRequest("La parcelle spécifiée n'existe pas ou n'appartient pas à votre société.");
            }

            // Vérifier que la nouvelle variété (si modifiée) appartient à la société
            if (produit.VarieteId != dto.VarieteId)
            {
                var variete = await _context.Varietes
                    .Include(v => v.Categorie)
                    .FirstOrDefaultAsync(v => v.Id == dto.VarieteId && v.Categorie.SocieteId == societeId);
                if (variete == null)
                    return BadRequest("La variété spécifiée n'existe pas ou n'appartient pas à votre société.");
            }

            produit.Nom = dto.Nom;
            produit.ParcelleId = dto.ParcelleId;
            produit.VarieteId = dto.VarieteId;

            await _context.SaveChangesAsync();

            // Recharger les relations pour le DTO de retour
            await _context.Entry(produit).Reference(p => p.Parcelle).LoadAsync();
            //await _context.Entry(produit).Reference(p => p.Variete).ThenInclude(v => v.Categorie).LoadAsync();

            var result = new ProduitDto
            {
                Id = produit.Id,
                Nom = produit.Nom,
                ParcelleId = produit.ParcelleId,
                ParcelleNom = produit.Parcelle?.NomParcelle,
                VarieteId = produit.VarieteId,
                VarieteNom = produit.Variete?.Intitule,
                CategorieNom = produit.Variete?.Categorie?.Intitule
            };

            return Ok(result);
        }

        // DELETE: api/Produits/5?societeId={societeId}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var produit = await _context.Produites
                .Include(p => p.Parcelle)
                    
                .FirstOrDefaultAsync(p => p.Id == id && p.SocieteId == societeId);

            if (produit == null)
                return NotFound("Produit non trouvé ou non autorisé.");

            _context.Produites.Remove(produit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Produits/byparcelle/{parcelleId}?societeId={societeId}
        // GET: api/Produits/byparcelle/{parcelleId}?societeId={societeId}
        [HttpGet("byparcelle/{parcelleId}")]
        public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduitByParcelle(
            int parcelleId,
            [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            // Vérifier que la parcelle existe et appartient à la société (propriété directe SocieteId)
            var parcelleValide = await _context.Parcelles
                .AnyAsync(p => p.Id == parcelleId && p.SocieteId == societeId);

            if (!parcelleValide)
                return NotFound("Parcelle non trouvée ou non autorisée.");

            var data = await _context.Produites
                .Where(p => p.ParcelleId == parcelleId)
                .Select(p => new ProduitDto
                {
                    Id = p.Id,
                    Nom = p.Nom,
                    ParcelleId = p.ParcelleId,
                    VarieteId = p.VarieteId
                })
                .ToListAsync();

            return Ok(data);
        }




        // GET: api/Produits/byvariete/{varieteId}?societeId={societeId}
        [HttpGet("byvariete/{varieteId}")]
        public async Task<ActionResult<VarieteDto>> GetVarieteByProduit(int varieteId, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var variete = await _context.Varietes
                .Include(v => v.Categorie)
                .FirstOrDefaultAsync(v => v.Id == varieteId && v.Categorie.SocieteId == societeId);

            if (variete == null)
                return NotFound("Variété non trouvée ou non autorisée.");

            var dto = new VarieteDto
            {
                Id = variete.Id,
                Intitule = variete.Intitule,
                CategorieId = variete.CategorieId,
                CategorieNom = variete.Categorie?.Intitule
            };

            return Ok(dto);
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
//    public class ProduitsController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public ProduitsController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // =========================
//        // GET ALL 
//        // api/Produits
//        // =========================
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll()
//        {
//            var data = await _context.Produites
//                .Include(p => p.Parcelle)
//                .Include(p => p.Variete)
//                    .ThenInclude(v => v.Categorie)
//                .Select(p => new ProduitDto
//                {
//                    Id = p.Id,
//                    Nom = p.Nom,

//                    ParcelleId = p.ParcelleId,
//                    ParcelleNom = p.Parcelle != null
//                        ? p.Parcelle.NomParcelle
//                        : "",

//                    VarieteId = p.VarieteId,
//                    VarieteNom = p.Variete != null
//                        ? p.Variete.Intitule
//                        : "",

//                    CategorieNom = p.Variete != null &&
//                                    p.Variete.Categorie != null
//                        ? p.Variete.Categorie.Intitule
//                        : ""
//                })
//                .ToListAsync();

//            return Ok(data);
//        }

//        // =========================
//        // GET BY ID
//        // api/Produits/5
//        // =========================
//        [HttpGet("{id}")]
//        public async Task<ActionResult<ProduitDto>> GetById(int id)
//        {
//            var p = await _context.Produites
//                .Include(x => x.Parcelle)
//                .Include(x => x.Variete)
//                    .ThenInclude(v => v.Categorie)
//                .FirstOrDefaultAsync(x => x.Id == id);

//            if (p == null)
//                return NotFound();

//            var dto = new ProduitDto
//            {
//                Id = p.Id,
//                Nom = p.Nom,

//                ParcelleId = p.ParcelleId,
//                ParcelleNom = p.Parcelle?.NomParcelle,

//                VarieteId = p.VarieteId,
//                VarieteNom = p.Variete?.Intitule,

//                CategorieNom = p.Variete?.Categorie?.Intitule
//            };

//            return Ok(dto);
//        }

//        // =========================
//        // CREATE
//        // api/Produits
//        // =========================
//        [HttpPost]
//        public async Task<ActionResult> Create(ProduitDto dto)
//        {
//            var produit = new Produit
//            {
//                Nom = dto.Nom,
//                ParcelleId = dto.ParcelleId,
//                VarieteId = dto.VarieteId
//            };

//            _context.Produites.Add(produit);

//            await _context.SaveChangesAsync();

//            return Ok(new ProduitDto
//            {
//                Id = produit.Id,
//                Nom = produit.Nom,
//                ParcelleId = produit.ParcelleId,
//                VarieteId = produit.VarieteId
//            });
//        }

//        // =========================
//        // UPDATE
//        // api/Produits/5
//        // =========================
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, ProduitDto dto)
//        {
//            var produit = await _context.Produites.FindAsync(id);

//            if (produit == null)
//                return NotFound();

//            produit.Nom = dto.Nom;
//            produit.ParcelleId = dto.ParcelleId;
//            produit.VarieteId = dto.VarieteId;

//            await _context.SaveChangesAsync();

//            return Ok(new ProduitDto
//            {
//                Id = produit.Id,
//                Nom = produit.Nom,
//                ParcelleId = produit.ParcelleId,
//                VarieteId = produit.VarieteId
//            });
//        }

//        // =========================
//        // DELETE
//        // api/Produits/5
//        // =========================
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var produit = await _context.Produites.FindAsync(id);

//            if (produit == null)
//                return NotFound();

//            _context.Produites.Remove(produit);

//            await _context.SaveChangesAsync();

//            return Ok();
//        }

//        // liste produit par parcelle 

//        [HttpGet("byparcelle/{parcelleid}")]
//        public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduitByparcelle(int parcelleid)
//        {
//            var data = await _context.Produites
//                .Where(f => f.ParcelleId == parcelleid)
//                .Select(f => new ProduitDto
//                {
//                    Id = f.Id,
//                    Nom = f.Nom,
//                    ParcelleId = f.ParcelleId,
//                    VarieteId = f.VarieteId

//                })
//                .ToListAsync();

//            return Ok(data);
//        }


//        // liste variete by produit                        

//        [HttpGet("byvariete/{varieteid}")]
//        public async Task<ActionResult<VarieteDto>> GetProduitByvariete(int varieteid)
//        {
//            var data = await _context.Varietes
//                .Include(v => v.Categorie)
//                .Where(v => v.Id == varieteid)
//                .Select(v => new VarieteDto
//                {
//                    Id = v.Id,
//                    Intitule = v.Intitule,
//                    CategorieId = v.CategorieId,
//                    CategorieNom = v.Categorie != null
//                        ? v.Categorie.Intitule
//                        : ""
//                })
//                .FirstOrDefaultAsync();

//            if (data == null)
//                return NotFound();

//            return Ok(data);
//        }


//    }
//}