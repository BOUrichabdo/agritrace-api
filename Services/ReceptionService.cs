using AgriTraceAPI.Data;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;
using TracAgriApi.Servises;

namespace TracAgriApi.Services
{
    public class ReceptionService : IReceptionService
    {
        private readonly AppDbContext _context;
        public ReceptionService(AppDbContext context)
        {
            _context = context;
        }
        // ReceptionService.cs - Version corrigée pour PostgreSQL
        public async Task<ReceptionResponseDto> CreateReceptionAsync(CreateReceptionDto dto)
        {

            // transaction pour cree reception 
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. chercher etiquette
                var etiquette = await _context.EtiquetteFermes
                    .Include(x => x.Produit)
                    .Include(x => x.Agriculteur)
                    .Include(x => x.Variete)
                    .FirstOrDefaultAsync(x => x.Id == dto.EtiquetteFermeId);

                if (etiquette == null)
                    throw new Exception("Etiquette introuvable");

                // 2. créer réception
                var reception = new Reception
                {
                    EtiquetteFermeId = dto.EtiquetteFermeId,
                    PoidsBrut = decimal.Round(dto.PoidsBrut, 2), // Arrondir pour PostgreSQL
                    Temperature = decimal.Round(dto.Temperature, 2),
                    EtatProduit = dto.EtatProduit,
                    TypeProduit = dto.TypeProduit,
                    Observation = dto.Observation ?? string.Empty,
                    Utilisateur = dto.Utilisateur ?? "admin",
                    DateReception = DateTime.UtcNow,
                    SocieteId = 1 // À ajuster selon votre logique
                };

                _context.Receptions.Add(reception);
                await _context.SaveChangesAsync();

                // 3. augmenter stock
                var stock = new Stock
                {
                    ReceptionId = reception.Id,
                    ProduitId = etiquette.ProduitId,
                    VarieteId = etiquette.VarieteId,
                    QuantiteDisponible = decimal.Round(reception.PoidsBrut, 2),
                    DateEntree = DateTime.UtcNow,
                    EtatStock = "Disponible"
                };

                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();

                // 4. créer palette
                var palette = new Palette
                {
                    CodePalette = $"PAL-{DateTime.Now:yyyyMMddHHmmss}",
                    ProduitId = etiquette.ProduitId,
                    PoidsBrut = decimal.Round(dto.PoidsBrut, 2),
                    QuantiteDisponible = decimal.Round(dto.PoidsBrut, 2),
                    EtatPalette = dto.EtatProduit,
                    StatutStock = "EN_STOCK",
                    DateCreation = DateTime.UtcNow,
                    Emplacement = "RECEPTION",
                    ReceptionId = reception.Id
                };

                _context.Palettes.Add(palette);
                await _context.SaveChangesAsync();

                // 5. lier réception → palette
                reception.PaletteId = palette.Id;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 6. retour API
                return new ReceptionResponseDto
                {
                    ReceptionId = reception.Id,
                    PaletteId = palette.Id,
                    CodePalette = palette.CodePalette,
                    PoidsBrut = palette.PoidsBrut,
                    QuantiteDisponible = palette.QuantiteDisponible,
                    DateReception = reception.DateReception,
                    Produit = etiquette.Produit?.Nom ?? string.Empty,
                    Agriculteur = etiquette.Agriculteur?.Nom ?? string.Empty,
                    CodeQR = etiquette.CodeEtiquette ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erreur lors de la création: {ex.Message}", ex);
            }
        }
    }
}
