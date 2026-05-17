using AgriTraceAPI.Data;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;
using TracAgriApi.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _context;

    public StockService(AppDbContext context)
    {
        _context = context;
    }
    // logique métier pour créer une sortie de stock à partir d'une palette
    public async Task<SortieStock> CreateSortieAsync(CreateSortieStockDto dto)
    {
        // 1. chercher réception via code palette
        var reception = await _context.Receptions
            .Include(r => r.Palette)
            .FirstOrDefaultAsync(r =>
                r.Palette != null &&
                r.Palette.CodePalette == dto.CodePalette);

        if (reception == null)
            throw new Exception("Palette introuvable");

        // 2. récupérer stock produit
        var stock = await _context.Stocks
      .FirstOrDefaultAsync(s =>
          s.ProduitId == reception.Palette!.ProduitId);

        if (stock == null)
            throw new Exception("Stock introuvable");

        // 3. contrôle stock
        if (stock.QuantiteDisponible < dto.QuantiteSortie)
            throw new Exception("Stock insuffisant");

        // 4. déduction stock
        stock.QuantiteDisponible -= dto.QuantiteSortie;

        // 5. créer sortie
        var sortie = new SortieStock
        {
            ReceptionId = reception.Id,
            QuantiteSortie = dto.QuantiteSortie,
            DateSortie = DateTime.Now,
            Utilisateur = dto.Utilisateur,
            Observation = dto.Observation
        };

        _context.SortieStocks.Add(sortie);

        await _context.SaveChangesAsync();

        return sortie;
    }

    // logique métier pour récupérer les détails d'une palette via son code
    public async Task<PaletteSortieDto?> GetPaletteByCodeAsync(string code)
    {
        var palette = await _context.Palettes

            .Include(p => p.Produit)
            .Include(p => p.Reception)
                .ThenInclude(r => r.EtiquetteFerme)
                    .ThenInclude(e => e.Variete)

            .FirstOrDefaultAsync(p => p.CodePalette == code);

        if (palette == null)
            return null;

        return new PaletteSortieDto
        {
            CodePalette = palette.CodePalette,

            Produit = palette.Produit != null
                ? palette.Produit.Nom
                : "",

            Variete = palette.Reception != null &&
                       palette.Reception.EtiquetteFerme != null &&
                       palette.Reception.EtiquetteFerme.Variete != null
                ? palette.Reception.EtiquetteFerme.Variete.Intitule
                : "",

            QuantiteDisponible = palette.QuantiteDisponible,

            EtatPalette = palette.EtatPalette
        };
    }
}