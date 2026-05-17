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
        public async Task<ReceptionResponseDto> CreateReceptionAsync(CreateReceptionDto dto)
        {
            // 1. chercher etiquette
            var etiquette = await _context.EtiquetteFermes
                .Include(x => x.Produit)
                .Include(x => x.Agriculteur)
                .FirstOrDefaultAsync(x => x.Id == dto.EtiquetteFermeId);

            if (etiquette == null)
                throw new Exception("Etiquette introuvable");

            // 2. créer réception
            var reception = new Reception
            {
                EtiquetteFermeId = dto.EtiquetteFermeId,
                PoidsBrut = dto.PoidsBrut,
                Temperature = dto.Temperature,
                EtatProduit = dto.EtatProduit,
                TypeProduit = dto.TypeProduit,
                Observation = dto.Observation, 
                Utilisateur = dto.Utilisateur,
                DateReception = DateTime.Now
            };

            _context.Receptions.Add(reception);
            await _context.SaveChangesAsync();
            // augmenter stock 
            var stock = new Stock
            {
                ReceptionId = reception.Id,

                ProduitId = etiquette.ProduitId,

                VarieteId = etiquette.VarieteId,

                QuantiteDisponible = reception.PoidsBrut,

                DateEntree = DateTime.Now,

                EtatStock = "Disponible"
            };

            _context.Stocks.Add(stock);

            await _context.SaveChangesAsync();
            // 3. créer palette
            var palette = new Palette
            {
                CodePalette = $"PAL-{DateTime.Now:yyyyMMddHHmmss}",
                ProduitId = etiquette.ProduitId,
                PoidsBrut = dto.PoidsBrut,
                QuantiteDisponible = dto.PoidsBrut,
                EtatPalette = dto.EtatProduit,
                StatutStock = "EN_STOCK",
                DateCreation = DateTime.Now,
                Emplacement = "RECEPTION",
                ReceptionId = reception.Id
            };

            _context.Palettes.Add(palette);
            await _context.SaveChangesAsync();

            // 4. lier réception → palette
            reception.PaletteId = palette.Id;
            await _context.SaveChangesAsync();

            // 5. retour API
            return new ReceptionResponseDto
            {
                ReceptionId = reception.Id,
                CodePalette = palette.CodePalette,
                PoidsBrut = palette.PoidsBrut,
                QuantiteDisponible = palette.QuantiteDisponible,
                DateReception = reception.DateReception,
                Produit = etiquette.Produit.Nom,
                Agriculteur = etiquette.Agriculteur.Nom,
                PaletteId = palette.Id,
            };
        }
    }
}
