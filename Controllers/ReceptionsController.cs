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


        // creation d'une reception a partir d'un DTO de creation
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReceptionDto dto)
        {
            try
            {
                var result = await _service.CreateReceptionAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
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
