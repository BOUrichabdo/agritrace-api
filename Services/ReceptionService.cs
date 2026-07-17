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
        // ReceptionService.cs - Version corrigée
        public async Task<ReceptionResponseDto> CreateReceptionAsync(CreateReceptionDto dto)
        {
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
                    throw new Exception($"Etiquette {dto.EtiquetteFermeId} introuvable");

                // 2. créer réception - ✅ AVEC SocieteId
                var reception = new Reception
                {
                    EtiquetteFermeId = dto.EtiquetteFermeId,
                    PoidsBrut = Math.Round(dto.PoidsBrut, 2),
                    Temperature = Math.Round(dto.Temperature, 2),
                    EtatProduit = dto.EtatProduit,
                    TypeProduit = dto.TypeProduit,
                    Observation = dto.Observation ?? string.Empty,
                    Utilisateur = dto.Utilisateur ?? "admin",
                    DateReception = DateTime.UtcNow,
                    SocieteId = dto.SocieteId > 0 ? dto.SocieteId : 1 // ✅ Valeur par défaut
                };

                _context.Receptions.Add(reception);
                await _context.SaveChangesAsync();

                // 3. augmenter stock
                var stock = new Stock
                {
                    ReceptionId = reception.Id,
                    ProduitId = etiquette.ProduitId,
                    VarieteId = etiquette.VarieteId,
                    QuantiteDisponible = Math.Round(reception.PoidsBrut, 2),
                    DateEntree = DateTime.UtcNow,
                    EtatStock = "Disponible",
                    SocieteId = dto.SocieteId > 0 ? dto.SocieteId : 1
                };

                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();

                // 4. créer palette
                var palette = new Palette
                {
                    CodePalette = $"PAL-{DateTime.Now:yyyyMMddHHmmss}",
                    ProduitId = etiquette.ProduitId,
                    PoidsBrut = Math.Round(dto.PoidsBrut, 2),
                    QuantiteDisponible = Math.Round(dto.PoidsBrut, 2),
                    EtatPalette = dto.EtatProduit,
                    StatutStock = "EN_STOCK",
                    DateCreation = DateTime.UtcNow,
                    Emplacement = "RECEPTION",
                    ReceptionId = reception.Id,
                    SocieteId = dto.SocieteId > 0 ? dto.SocieteId : 1
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
                    Temperature = reception.Temperature,
                    Etat = reception.EtatProduit,
                    Type = reception.TypeProduit,
                    Observation = reception.Observation,
                    Produit = etiquette.Produit?.Nom ?? string.Empty,
                    Agriculteur = etiquette.Agriculteur?.Nom ?? string.Empty,
                    Ferme = etiquette.Ferme?.NomFerme ?? string.Empty,
                    Variete = etiquette.Variete?.Intitule ?? string.Empty,
                    CodeQR = etiquette.CodeEtiquette ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"ERREUR CreateReception: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}
