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

        // ===========================
        // GENERER ETIQUETTE (CORRIGÉ)
        // ===========================
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
                    DateGeneration = DateTime.Now,
                    Receptionne = false,
                    SocieteId = 1
                };

                _context.EtiquetteFermes.Add(etiquette);
                await _context.SaveChangesAsync();

                // 2. Récupérer avec les relations (VERSION SÉCURISÉE)
                var result = await _context.EtiquetteFermes
                    .Where(e => e.Id == etiquette.Id)
                    .Select(e => new EtiquetteFermeDto
                    {
                        Id = e.Id,
                        CodeEtiquette = e.CodeEtiquette,

                        AgriculteurId = e.AgriculteurId,
                        // ✅ VÉRIFICATION NULL
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
                    .FirstOrDefaultAsync();

                if (result == null)
                    return NotFound(new { message = "Erreur lors de la génération" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Capture l'erreur exacte pour debug
                return StatusCode(500, new
                {
                    message = "Erreur lors de la génération de l'étiquette",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message



                });
            }
        }

    }
}