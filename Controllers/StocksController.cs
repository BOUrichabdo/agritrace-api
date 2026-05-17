using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;
using TracAgriApi.Services;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _context;
        // implimentation du service de stock pour la gestion des palettes
        private readonly IStockService _service;

        public StocksController(AppDbContext context , IStockService service)
        {
            _context = context;

            _service = service;
        }

        // =========================
        // AJOUT STOCK  module reception 
        // =========================

        [HttpPost]
        public async Task<IActionResult> Create(CreateStockDto dto)
        {
            var stock = new Stock
            {
                ReceptionId = dto.ReceptionId,

                ProduitId = dto.ProduitId,

                VarieteId = dto.VarieteId,

                QuantiteDisponible = dto.QuantiteDisponible,

                Emplacement = dto.Emplacement,

                DateEntree = DateTime.Now,

                EtatStock = "Disponible"
            };

            _context.Stocks.Add(stock);

            await _context.SaveChangesAsync();

            return Ok(new StockDto
            {
                Id = stock.Id,

                ReceptionId = stock.ReceptionId,

                QuantiteDisponible = stock.QuantiteDisponible,

                DateEntree = stock.DateEntree,

                EtatStock = stock.EtatStock,

                Emplacement = stock.Emplacement ?? ""
            });
        }

        // =========================
        // LISTE STOCK
        // =========================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
        {
            var data = await _context.Stocks

                .Include(s => s.Produit)
                .Include(s => s.Variete)

                .Select(s => new StockDto
                {
                    Id = s.Id,

                    ReceptionId = s.ReceptionId,

                    Produit = s.Produit != null
                        ? s.Produit.Nom
                        : "",

                    Variete = s.Variete != null
                        ? s.Variete.Intitule
                        : "",

                    QuantiteDisponible = s.QuantiteDisponible,

                    DateEntree = s.DateEntree,

                    EtatStock = s.EtatStock,

                    Emplacement = s.Emplacement
                })

                .ToListAsync();

            return Ok(data);
        }


        // ===================================================
        // GET PALETTE PAR CODE
        // ===================================================

        [HttpGet("palette/{code}")]
        public async Task<IActionResult> GetPalette(string code)
        {
            try
            {
                var result = await _service.GetPaletteByCodeAsync(code);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Palette introuvable"
                    });
                }

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
    }
}
