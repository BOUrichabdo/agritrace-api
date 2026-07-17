using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtiquetteFermeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EtiquetteFermeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EtiquetteFerme?societeId={societeId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EtiquetteFermeDto>>> GetAll([FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var data = await _context.EtiquetteFermes
                .Include(e => e.Agriculteur)
                .Include(e => e.Ferme)
                .Include(e => e.Parcelle)
                .Include(e => e.Produit)
                .Include(e => e.Variete)
                .Where(e => e.Agriculteur.SocieteId == societeId) // 🔥 filtre via Agriculteur
                .Select(e => new EtiquetteFermeDto
                {
                    Id = e.Id,
                    CodeEtiquette = e.CodeEtiquette,
                    AgriculteurId = e.AgriculteurId,
                    AgriculteurNom = e.Agriculteur != null ? e.Agriculteur.Nom : "Non renseigné",
                    FermeId = e.FermeId,
                    FermeNom = e.Ferme != null ? e.Ferme.NomFerme : "Non renseigné",
                    ParcelleId = e.ParcelleId,
                    ParcelleNom = e.Parcelle != null ? e.Parcelle.NomParcelle : "Non renseigné",
                    ProduitId = e.ProduitId,
                    ProduitNom = e.Produit != null ? e.Produit.Nom : "Non renseigné",
                    VarieteId = e.VarieteId,
                    VarieteNom = e.Variete != null ? e.Variete.Intitule : "Non renseigné",
                    DateGeneration = e.DateGeneration,
                    Receptionne = e.Receptionne,
                    DateReception = e.DateReception
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/EtiquetteFerme/{id}?societeId={societeId}
        [HttpGet("{id}")]
        public async Task<ActionResult<EtiquetteFermeDto>> GetById(int id, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var etiquette = await _context.EtiquetteFermes
                .Include(e => e.Agriculteur)
                .Include(e => e.Ferme)
                .Include(e => e.Parcelle)
                .Include(e => e.Produit)
                .Include(e => e.Variete)
                .FirstOrDefaultAsync(e => e.Id == id && e.Agriculteur.SocieteId == societeId);

            if (etiquette == null)
                return NotFound(new { message = "Étiquette introuvable ou non autorisée." });

            var result = new EtiquetteFermeDto
            {
                Id = etiquette.Id,
                CodeEtiquette = etiquette.CodeEtiquette,
                AgriculteurId = etiquette.AgriculteurId,
                AgriculteurNom = etiquette.Agriculteur?.Nom ?? "Non renseigné",
                FermeId = etiquette.FermeId,
                FermeNom = etiquette.Ferme?.NomFerme ?? "Non renseigné",
                ParcelleId = etiquette.ParcelleId,
                ParcelleNom = etiquette.Parcelle?.NomParcelle ?? "Non renseigné",
                ProduitId = etiquette.ProduitId,
                ProduitNom = etiquette.Produit?.Nom ?? "Non renseigné",
                VarieteId = etiquette.VarieteId,
                VarieteNom = etiquette.Variete?.Intitule ?? "Non renseigné",
                DateGeneration = etiquette.DateGeneration,
                Receptionne = etiquette.Receptionne,
                DateReception = etiquette.DateReception
            };

            return Ok(result);
        }

        // GET: api/EtiquetteFerme/bycode/{code}?societeId={societeId}
        [HttpGet("bycode/{code}")]
        public async Task<ActionResult<EtiquetteFermeDto>> GetByCode(string code, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            var etiquette = await _context.EtiquetteFermes
                .Include(e => e.Agriculteur)
                .Include(e => e.Ferme)
                .Include(e => e.Parcelle)
                .Include(e => e.Produit)
                .Include(e => e.Variete)
                .FirstOrDefaultAsync(e => e.CodeEtiquette == code && e.Agriculteur.SocieteId == societeId);

            if (etiquette == null)
                return NotFound(new { message = "Étiquette introuvable ou non autorisée." });

            var result = new EtiquetteFermeDto
            {
                Id = etiquette.Id,
                CodeEtiquette = etiquette.CodeEtiquette,
                AgriculteurId = etiquette.AgriculteurId,
                AgriculteurNom = etiquette.Agriculteur?.Nom ?? "Non renseigné",
                FermeId = etiquette.FermeId,
                FermeNom = etiquette.Ferme?.NomFerme ?? "Non renseigné",
                ParcelleId = etiquette.ParcelleId,
                ParcelleNom = etiquette.Parcelle?.NomParcelle ?? "Non renseigné",
                ProduitId = etiquette.ProduitId,
                ProduitNom = etiquette.Produit?.Nom ?? "Non renseigné",
                VarieteId = etiquette.VarieteId,
                VarieteNom = etiquette.Variete?.Intitule ?? "Non renseigné",
                DateGeneration = etiquette.DateGeneration,
                Receptionne = etiquette.Receptionne,
                DateReception = etiquette.DateReception
            };

            return Ok(result);
        }

        // POST: api/EtiquetteFerme/generer?societeId={societeId}
        [HttpPost("generer")]
        public async Task<ActionResult<EtiquetteFermeDto>> Generer(CreateEtiquetteFermeDto dto, [FromQuery] int societeId)
        {
            if (societeId <= 0)
                return BadRequest("SocieteId invalide.");

            try
            {
                // Vérifier que l'agriculteur appartient à la société
                var agriculteur = await _context.Agriculteurs
                    .FirstOrDefaultAsync(a => a.Id == dto.AgriculteurId && a.SocieteId == societeId);
                if (agriculteur == null)
                    return BadRequest("Agriculteur invalide ou n'appartient pas à la société.");

                // Vérifier que la ferme appartient à l'agriculteur
                var ferme = await _context.Fermes
                    .FirstOrDefaultAsync(f => f.Id == dto.FermeId && f.AgriculteurId == dto.AgriculteurId);
                if (ferme == null)
                    return BadRequest("Ferme invalide ou n'appartient pas à l'agriculteur.");

                // Vérifier que la parcelle appartient à l'agriculteur
                //var parcelle = await _context.Parcelles
                //    .FirstOrDefaultAsync(p => p.Id == dto.ParcelleId && p.AgriculteurId. == dto.AgriculteurId);
                //if (parcelle == null)
                //    return BadRequest("Parcelle invalide ou n'appartient pas à l'agriculteur.");

                // Vérifier que le produit appartient à la parcelle
                var produit = await _context.Produites
                    .FirstOrDefaultAsync(p => p.Id == dto.ProduitId && p.ParcelleId == dto.ParcelleId);
                if (produit == null)
                    return BadRequest("Produit invalide ou n'appartient pas à la parcelle.");

                // Vérifier que la variété appartient à la société (via sa catégorie)
                var variete = await _context.Varietes
                    .Include(v => v.Categorie)
                    .FirstOrDefaultAsync(v => v.Id == dto.VarieteId && v.Categorie.SocieteId == societeId);
                if (variete == null)
                    return BadRequest("Variété invalide ou n'appartient pas à la société.");

                // Créer l'étiquette
                var etiquette = new EtiquetteFerme
                {
                    CodeEtiquette = dto.CodeEtiquette,
                    AgriculteurId = dto.AgriculteurId,
                    FermeId = dto.FermeId,
                    ParcelleId = dto.ParcelleId,
                    ProduitId = dto.ProduitId,
                    VarieteId = dto.VarieteId,
                    DateGeneration = DateTime.UtcNow,
                    Receptionne = false,
                    SocieteId = societeId // 🔥 on stocke l'ID de la société dans l'étiquette
                };

                _context.EtiquetteFermes.Add(etiquette);
                await _context.SaveChangesAsync();

                // Recharger avec les relations pour le DTO
                var etiquetteChargee = await _context.EtiquetteFermes
                    .Include(e => e.Agriculteur)
                    .Include(e => e.Ferme)
                    .Include(e => e.Parcelle)
                    .Include(e => e.Produit)
                    .Include(e => e.Variete)
                    .FirstOrDefaultAsync(e => e.Id == etiquette.Id);

                if (etiquetteChargee == null)
                    return NotFound(new { message = "Étiquette non trouvée après création." });

                var result = new EtiquetteFermeDto
                {
                    Id = etiquetteChargee.Id,
                    CodeEtiquette = etiquetteChargee.CodeEtiquette,
                    AgriculteurId = etiquetteChargee.AgriculteurId,
                    AgriculteurNom = etiquetteChargee.Agriculteur?.Nom ?? "Non renseigné",
                    FermeId = etiquetteChargee.FermeId,
                    FermeNom = etiquetteChargee.Ferme?.NomFerme ?? "Non renseigné",
                    ParcelleId = etiquetteChargee.ParcelleId,
                    ParcelleNom = etiquetteChargee.Parcelle?.NomParcelle ?? "Non renseigné",
                    ProduitId = etiquetteChargee.ProduitId,
                    ProduitNom = etiquetteChargee.Produit?.Nom ?? "Non renseigné",
                    VarieteId = etiquetteChargee.VarieteId,
                    VarieteNom = etiquetteChargee.Variete?.Intitule ?? "Non renseigné",
                    DateGeneration = etiquetteChargee.DateGeneration,
                    Receptionne = etiquetteChargee.Receptionne,
                    DateReception = etiquetteChargee.DateReception
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur lors de la génération de l'étiquette.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
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
//    public class EtiquetteFermeController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public EtiquetteFermeController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("generer")]
//        public async Task<ActionResult<EtiquetteFermeDto>> Generer(CreateEtiquetteFermeDto dto)
//        {
//            try
//            {
//                // 1. Créer l'étiquette
//                var etiquette = new EtiquetteFerme
//                {
//                    CodeEtiquette = dto.CodeEtiquette,
//                    AgriculteurId = dto.AgriculteurId,
//                    FermeId = dto.FermeId,
//                    ParcelleId = dto.ParcelleId,
//                    ProduitId = dto.ProduitId,
//                    VarieteId = dto.VarieteId,
//                    DateGeneration = DateTime.UtcNow,
//                    Receptionne = false,
//                    SocieteId = 1
//                };

//                _context.EtiquetteFermes.Add(etiquette);
//                await _context.SaveChangesAsync();

//                // 2. Recharger avec les relations (Include)
//                var etiquetteChargee = await _context.EtiquetteFermes
//                    .Include(e => e.Agriculteur)
//                    .Include(e => e.Ferme)
//                    .Include(e => e.Parcelle)
//                    .Include(e => e.Produit)
//                    .Include(e => e.Variete)
//                    .FirstOrDefaultAsync(e => e.Id == etiquette.Id);

//                if (etiquetteChargee == null)
//                    return NotFound(new { message = "Étiquette non trouvée" });

//                // 3. Mapper vers DTO
//                var result = new EtiquetteFermeDto
//                {
//                    Id = etiquetteChargee.Id,
//                    CodeEtiquette = etiquetteChargee.CodeEtiquette,
//                    AgriculteurId = etiquetteChargee.AgriculteurId,
//                    AgriculteurNom = etiquetteChargee.Agriculteur?.Nom ?? "Non renseigné",
//                    FermeId = etiquetteChargee.FermeId,
//                    FermeNom = etiquetteChargee.Ferme?.NomFerme ?? "Non renseigné",
//                    ParcelleId = etiquetteChargee.ParcelleId,
//                    ParcelleNom = etiquetteChargee.Parcelle?.NomParcelle ?? "Non renseigné",
//                    ProduitId = etiquetteChargee.ProduitId,
//                    ProduitNom = etiquetteChargee.Produit?.Nom ?? "Non renseigné",
//                    VarieteId = etiquetteChargee.VarieteId,
//                    VarieteNom = etiquetteChargee.Variete?.Intitule ?? "Non renseigné",
//                    DateGeneration = etiquetteChargee.DateGeneration,
//                    Receptionne = etiquetteChargee.Receptionne,
//                    DateReception = etiquetteChargee.DateReception
//                };

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    message = "Erreur lors de la génération",
//                    error = ex.Message,
//                    innerError = ex.InnerException?.Message
//                });
//            }
//        }

//        [HttpGet("bycode/{code}")]
//        public async Task<ActionResult<EtiquetteFermeDto>> GetByCode(string code)
//        {
//            var etiquette = await _context.EtiquetteFermes
//                .Include(e => e.Agriculteur)
//                .Include(e => e.Ferme)
//                .Include(e => e.Parcelle)
//                .Include(e => e.Produit)
//                .Include(e => e.Variete)
//                .FirstOrDefaultAsync(e => e.CodeEtiquette == code);

//            if (etiquette == null)
//                return NotFound(new { message = "Étiquette introuvable" });

//            var result = new EtiquetteFermeDto
//            {
//                Id = etiquette.Id,
//                CodeEtiquette = etiquette.CodeEtiquette,
//                AgriculteurId = etiquette.AgriculteurId,
//                AgriculteurNom = etiquette.Agriculteur?.Nom ?? "Non renseigné",
//                FermeId = etiquette.FermeId,
//                FermeNom = etiquette.Ferme?.NomFerme ?? "Non renseigné",
//                ParcelleId = etiquette.ParcelleId,
//                ParcelleNom = etiquette.Parcelle?.NomParcelle ?? "Non renseigné",
//                ProduitId = etiquette.ProduitId,
//                ProduitNom = etiquette.Produit?.Nom ?? "Non renseigné",
//                VarieteId = etiquette.VarieteId,
//                VarieteNom = etiquette.Variete?.Intitule ?? "Non renseigné",
//                DateGeneration = etiquette.DateGeneration,
//                Receptionne = etiquette.Receptionne,
//                DateReception = etiquette.DateReception
//            };

//            return Ok(result);
//        }
//    }
//}