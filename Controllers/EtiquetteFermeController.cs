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

        // ===========================
        // GET BY CODE (CORRIGÉ)
        // ===========================
        [HttpGet("bycode/{code}")]
        public async Task<ActionResult<EtiquetteFermeDto>> GetByCode(string code)
        {
            var etiquette = await _context.EtiquetteFermes
                .Where(e => e.CodeEtiquette == code)
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
                .FirstOrDefaultAsync();

            if (etiquette == null)
                return NotFound(new { message = "Étiquette introuvable" });

            return Ok(etiquette);
        }
    }
}














//using AgriTraceAPI.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TracAgriApi.DTOs;
//using TracAgriApi.Models;
//using TracAgriApi.Services;

//namespace TracAgriApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EtiquetteFermeController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        //private readonly PdfService _pdfService;
//        public EtiquetteFermeController(AppDbContext context)
//        {
//            _context = context;
//            //_pdfService = pdfService;

//        }
//        // ===========================
//        // GENERER ETIQUETTE
//        // ===========================
//        [HttpPost("generer")]
//        public async Task<ActionResult<EtiquetteFermeDto>> Generer(
//            CreateEtiquetteFermeDto dto)
//        {
//            // 🔢 code unique par code produit
//            //string code =
//            //    $"ETQ-{dto.ProduitId}";

//            var etiquette = new EtiquetteFerme
//            {
//                CodeEtiquette = dto.CodeEtiquette,
//                AgriculteurId = dto.AgriculteurId,
//                FermeId = dto.FermeId,
//                ParcelleId = dto.ParcelleId,
//                ProduitId = dto.ProduitId,
//                VarieteId = dto.VarieteId,
//                DateGeneration = DateTime.Now,
//                Receptionne = false,
//                SocieteId = 1
//            };

//            _context.EtiquetteFermes.Add(etiquette);

//            await _context.SaveChangesAsync();

//            // =========================
//            // RETURN DTO
//            // =========================

//            var result = await _context.EtiquetteFermes
//                .Include(e => e.Agriculteur)
//                .Include(e => e.Ferme)
//                .Include(e => e.Parcelle)
//                .Include(e => e.Produit)
//                .Include(e => e.Variete)

//                .Where(e => e.Id == etiquette.Id)

//                .Select(e => new EtiquetteFermeDto
//                {
//                    Id = e.Id,

//                    CodeEtiquette = e.CodeEtiquette,

//                    AgriculteurId = e.AgriculteurId,
//                    AgriculteurNom = e.Agriculteur!.Nom,

//                    FermeId = e.FermeId,
//                    FermeNom = e.Ferme!.NomFerme,

//                    ParcelleId = e.ParcelleId,
//                    ParcelleNom = e.Parcelle!.NomParcelle,

//                    ProduitId = e.ProduitId,
//                    ProduitNom = e.Produit!.Nom,

//                    VarieteId = e.VarieteId,
//                    VarieteNom = e.Variete!.Intitule,

//                    DateGeneration = e.DateGeneration,

//                    Receptionne = e.Receptionne,

//                    DateReception = e.DateReception
//                })

//                .FirstAsync();

//            return Ok(result);
//        }


//        // lorsque tester dans swager lorsque naviguer par swagger dans plat form Railway afficher erreur 

//        [HttpGet("bycode/{code}")]
//        public async Task<ActionResult<EtiquetteFermeDto>> GetByCode(string code)
//        {
//            var etiquette = await _context.EtiquetteFermes

//                .Include(e => e.Agriculteur)
//                .Include(e => e.Ferme)
//                .Include(e => e.Parcelle)
//                .Include(e => e.Produit)
//                .Include(e => e.Variete)

//                .Where(e => e.CodeEtiquette == code)

//                .Select(e => new EtiquetteFermeDto
//                {
//                    Id = e.Id,

//                    CodeEtiquette = e.CodeEtiquette,

//                    AgriculteurId = e.AgriculteurId,
//                    AgriculteurNom = e.Agriculteur!.Nom,

//                    FermeId = e.FermeId,
//                    FermeNom = e.Ferme!.NomFerme,

//                    ParcelleId = e.ParcelleId,
//                    ParcelleNom = e.Parcelle!.NomParcelle,

//                    ProduitId = e.ProduitId,
//                    ProduitNom = e.Produit!.Nom,

//                    VarieteId = e.VarieteId,
//                    VarieteNom = e.Variete!.Intitule,

//                    DateGeneration = e.DateGeneration,

//                    Receptionne = e.Receptionne,

//                    DateReception = e.DateReception
//                })

//                .FirstOrDefaultAsync();

//            if (etiquette == null)
//                return NotFound(new
//                {
//                    message = "Etiquette introuvable"
//                });

//            return Ok(etiquette);

//        }




//    }
//}