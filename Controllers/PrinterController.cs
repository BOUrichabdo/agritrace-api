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
            try
            {
                Console.WriteLine($"=== PRINT PDF START ID: {id} ===");

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
                        AgriculteurNom = e.Agriculteur != null ? e.Agriculteur.Nom : "Non renseigné",
                        FermeNom = e.Ferme != null ? e.Ferme.NomFerme : "Non renseigné",
                        ParcelleNom = e.Parcelle != null ? e.Parcelle.NomParcelle : "Non renseigné",
                        ProduitNom = e.Produit != null ? e.Produit.Nom : "Non renseigné",
                        VarieteNom = e.Variete != null ? e.Variete.Intitule : "Non renseigné",
                        DateGeneration = e.DateGeneration
                    })
                    .FirstOrDefaultAsync();

                if (etiquette == null)
                {
                    Console.WriteLine($"Erreur: Étiquette ID {id} non trouvée");
                    return NotFound(new { message = $"Étiquette ID {id} non trouvée" });
                }

                Console.WriteLine($"Étiquette trouvée: {etiquette.CodeEtiquette}");

                // Générer QR
                var qrBytes = _qrService.Generate(etiquette.CodeEtiquette);
                etiquette.QrCode = qrBytes;
                Console.WriteLine($"QR généré: {qrBytes?.Length ?? 0} bytes");

                // Générer PDF
                var pdf = _pdfService.Generate(etiquette);
                Console.WriteLine($"PDF généré: {pdf?.Length ?? 0} bytes");

                if (pdf == null || pdf.Length == 0)
                    return StatusCode(500, new { message = "Génération PDF vide" });

                return File(pdf, "application/pdf", $"etiquette_{id}.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                return StatusCode(500, new
                {
                    message = "Erreur lors de la génération du PDF",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("palette/{id}")]
        public async Task<IActionResult> PrintPalette(int id)
        {
            try
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
                    return NotFound(new { message = $"Palette ID {id} non trouvée" });

                var etiq = palette.Reception?.EtiquetteFerme;
                if (etiq == null)
                    return NotFound(new { message = "Étiquette associée non trouvée" });

                var dto = new PalettePrintDto
                {
                    Id = palette.Id,
                    CodePalette = palette.CodePalette ?? "",
                    // ✅ Vérifications null
                    AgriculteurNom = etiq.Agriculteur?.Nom ?? "Non renseigné",
                    FermeNom = etiq.Ferme?.NomFerme ?? "Non renseigné",
                    ProduitNom = etiq.Produit?.Nom ?? "Non renseigné",
                    VarieteNom = etiq.Variete?.Intitule ?? "Non renseigné",
                    PoidsBrut = palette.PoidsBrut,
                    DateCreation = palette.DateCreation,
                    QrCode = _qrService?.Generate(palette.CodePalette ?? "")
                };

                var pdf = _pdfService?.GeneratePalette(dto);
                if (pdf == null || pdf.Length == 0)
                    return StatusCode(500, new { message = "Génération PDF palette vide" });

                return File(pdf, "application/pdf", $"palette_{id}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erreur lors de la génération du PDF palette",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }
}