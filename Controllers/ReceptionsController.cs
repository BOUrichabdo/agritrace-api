using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Servises;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionsController : ControllerBase
    {
        // contrat interface Reception service pour deleguer la logique metier a une classe de service
        private readonly IReceptionService _service;

        private readonly AppDbContext _dbContext;
        public ReceptionsController(IReceptionService service, AppDbContext dbContext)
        {
            _service = service;
            _dbContext = dbContext;
        }


        // ReceptionsController.cs - Ajoutez du logging
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReceptionDto dto)
        {
            try
            {
                Console.WriteLine($"=== DEBUG: CreateReception ===");
                Console.WriteLine($"EtiquetteFermeId: {dto.EtiquetteFermeId}");
                Console.WriteLine($"PoidsBrut: {dto.PoidsBrut}");
                Console.WriteLine($"Temperature: {dto.Temperature}");
                Console.WriteLine($"EtatProduit: {dto.EtatProduit}");
                Console.WriteLine($"TypeProduit: {dto.TypeProduit}");

                var result = await _service.CreateReceptionAsync(dto);

                Console.WriteLine($"Succès: PaletteId={result.PaletteId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR: {ex.Message}");
                Console.WriteLine($"STACK: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    message = ex.Message,
                    details = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReceptions()
        {
            var receptions = await _dbContext.Receptions
                .Include(r => r.EtiquetteFerme)
                    .ThenInclude(e => e.Agriculteur)
                .Include(r => r.EtiquetteFerme)
                    .ThenInclude(e => e.Ferme)
                .Include(r => r.EtiquetteFerme)
                    .ThenInclude(e => e.Produit)
                .Include(r => r.EtiquetteFerme)
                    .ThenInclude(e => e.Variete)
                .Include(r => r.Palette)
                .Select(r => new ReceptionResponseDto
                {
                    ReceptionId = r.Id,
                    PaletteId = r.PaletteId ?? 0,
                    CodePalette = r.Palette != null ? r.Palette.CodePalette : string.Empty,
                    PoidsBrut = r.PoidsBrut,
                    QuantiteDisponible = r.Palette != null ? r.Palette.QuantiteDisponible : 0,
                    DateReception = r.DateReception,
                    Temperature = r.Temperature,
                    Etat = r.EtatProduit,
                    Type = r.TypeProduit,
                    Observation = r.Observation,
                    CodeQR = r.EtiquetteFerme != null ? r.EtiquetteFerme.CodeEtiquette : string.Empty,
                    Agriculteur = r.EtiquetteFerme != null && r.EtiquetteFerme.Agriculteur != null
                        ? r.EtiquetteFerme.Agriculteur.Nom
                        : string.Empty,
                    Ferme = r.EtiquetteFerme != null && r.EtiquetteFerme.Ferme != null
                        ? r.EtiquetteFerme.Ferme.NomFerme
                        : string.Empty,
                    Produit = r.EtiquetteFerme != null && r.EtiquetteFerme.Produit != null
                        ? r.EtiquetteFerme.Produit.Nom
                        : string.Empty,
                    Variete = r.EtiquetteFerme != null && r.EtiquetteFerme.Variete != null
                        ? r.EtiquetteFerme.Variete.Intitule
                        : string.Empty
                })
                .ToListAsync();

            return Ok(receptions);
        }


    }


}
