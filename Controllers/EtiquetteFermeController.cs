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

        [HttpPost("generer")]
        public async Task<ActionResult<EtiquetteFermeDto>> Generer(CreateEtiquetteFermeDto dto)
        {
            try
            {
                // 1. Créer l'étiquette
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
                    SocieteId = 1
                };

                _context.EtiquetteFermes.Add(etiquette);
                await _context.SaveChangesAsync();

                // 2. Recharger avec les relations (Include)
                var etiquetteChargee = await _context.EtiquetteFermes
                    .Include(e => e.Agriculteur)
                    .Include(e => e.Ferme)
                    .Include(e => e.Parcelle)
                    .Include(e => e.Produit)
                    .Include(e => e.Variete)
                    .FirstOrDefaultAsync(e => e.Id == etiquette.Id);

                if (etiquetteChargee == null)
                    return NotFound(new { message = "Étiquette non trouvée" });

                // 3. Mapper vers DTO
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
                    message = "Erreur lors de la génération",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("bycode/{code}")]
        public async Task<ActionResult<EtiquetteFermeDto>> GetByCode(string code)
        {
            var etiquette = await _context.EtiquetteFermes
                .Include(e => e.Agriculteur)
                .Include(e => e.Ferme)
                .Include(e => e.Parcelle)
                .Include(e => e.Produit)
                .Include(e => e.Variete)
                .FirstOrDefaultAsync(e => e.CodeEtiquette == code);

            if (etiquette == null)
                return NotFound(new { message = "Étiquette introuvable" });

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
    }
}