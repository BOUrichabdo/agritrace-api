using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Services;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrinterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PdfService _pdfService;
        private readonly QrService _qrService;


        public PrinterController(
         AppDbContext context,
         PdfService pdfService,
         QrService qrService)
        {
            _context = context;
            _pdfService = pdfService;
            _qrService = qrService;
        }

        [HttpGet("print/{id}")]
        public async Task<IActionResult> Print(int id)
        {
            var etiquette = await _context.EtiquetteFermes
                .Include(e => e.Agriculteur)
                .Include(e => e.Ferme)
                .Include(e => e.Parcelle)
                .Include(e => e.Produit)
                .Include(e => e.Variete)
                .Where(e => e.Id == id)
                .Select(e => new EtiquetteFermeDto
                {
                    Id = e.Id,
                    CodeEtiquette = e.CodeEtiquette,
                    AgriculteurNom = e.Agriculteur!.Nom,
                    FermeNom = e.Ferme!.NomFerme,
                    ParcelleNom = e.Parcelle!.NomParcelle,
                    ProduitNom = e.Produit!.Nom,
                    VarieteNom = e.Variete!.Intitule,
                    DateGeneration = e.DateGeneration
                })
                .FirstOrDefaultAsync();

            if (etiquette == null)
                return NotFound();

            // 🔥 GENERER QR
            var qrBytes = _qrService.Generate(etiquette.CodeEtiquette);
            etiquette.QrCode = qrBytes;

            // 🔥 GENERER PDF AVEC QR
            var pdf = _pdfService.Generate(etiquette);

            return File(pdf, "application/pdf", $"etiquette_{id}.pdf");
        }



        [HttpGet("palette/{id}")]
        public async Task<IActionResult> PrintPalette(int id)
        {
            var palette = await _context.Palettes
                .Include(p => p.Reception)
                    .ThenInclude(r => r.EtiquetteFerme)
                        .ThenInclude(e => e.Agriculteur)
                .Include(p => p.Reception)
                    .ThenInclude(r => r.EtiquetteFerme)
                        .ThenInclude(e => e.Ferme)
                .Include(p => p.Reception)
                    .ThenInclude(r => r.EtiquetteFerme)
                        .ThenInclude(e => e.Produit)
                .Include(p => p.Reception)
                    .ThenInclude(r => r.EtiquetteFerme)
                        .ThenInclude(e => e.Variete)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (palette == null)
                return NotFound();

            var etiq = palette.Reception.EtiquetteFerme;

            var dto = new PalettePrintDto
            {
                Id = palette.Id,
                CodePalette = palette.CodePalette,
                AgriculteurNom = etiq.Agriculteur.Nom,
                FermeNom = etiq.Ferme.NomFerme,
                ProduitNom = etiq.Produit.Nom,
                VarieteNom = etiq.Variete.Intitule,
                PoidsBrut = palette.PoidsBrut,
                DateCreation = palette.DateCreation,

                QrCode = _qrService.Generate(palette.CodePalette)
            };

            var pdf = _pdfService.GeneratePalette(dto);

            return File(pdf, "application/pdf", $"palette_{id}.pdf");
        }
    }
}