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


    public async Task<SortieStock> CreateSortieAsync(CreateSortieStockDto dto)
    {
        // 1. Récupérer la réception avec sa palette
        var reception = await _context.Receptions
            .Include(r => r.Palette)
            .FirstOrDefaultAsync(r =>
                r.Palette != null &&
                r.Palette.CodePalette == dto.CodePalette);

        if (reception == null)
            throw new Exception("Palette introuvable");

        // 2. Récupérer le stock lié à cette réception
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ReceptionId == reception.Id);

        if (stock == null)
            throw new Exception("Stock introuvable pour cette réception");

        // 3. Vérifier la quantité disponible
        if (stock.QuantiteDisponible < dto.QuantiteSortie)
            throw new Exception($"Stock insuffisant (disponible : {stock.QuantiteDisponible} KG)");

        // 4. Mettre à jour le stock
        stock.QuantiteDisponible -= dto.QuantiteSortie;

        // 5. Mettre à jour la quantité de la palette
        var palette = reception.Palette;
        palette.QuantiteDisponible -= dto.QuantiteSortie;
        if (palette.QuantiteDisponible == 0)
            palette.EtatPalette = "Vide";

        // 6. Déterminer SocieteId
        var societeId = stock.SocieteId != 0 ? stock.SocieteId : reception.SocieteId;
        if (societeId == 0)
            throw new Exception("SocieteId introuvable");

        // 7. Créer la sortie
        var sortie = new SortieStock
        {
            ReceptionId = reception.Id,
            QuantiteSortie = dto.QuantiteSortie,
            DateSortie = DateTime.UtcNow,
            Utilisateur = dto.Utilisateur,
            Observation = dto.Observation,
            SocieteId = societeId
        };

        _context.SortieStocks.Add(sortie);
        await _context.SaveChangesAsync();

        return sortie;
    }


    //public async Task<SortieStock> CreateSortieAsync(CreateSortieStockDto dto)
    //{
    //    // 1. récupérer la réception avec sa palette
    //    var reception = await _context.Receptions
    //        .Include(r => r.Palette)
    //        .FirstOrDefaultAsync(r => r.Palette != null && r.Palette.CodePalette == dto.CodePalette);

    //    if (reception == null)
    //        throw new Exception("Palette introuvable");

    //    // 2. récupérer le stock produit
    //    var stock = await _context.Stocks
    //        .FirstOrDefaultAsync(s => s.ProduitId == reception.Palette!.ProduitId);

    //    if (stock == null)
    //        throw new Exception("Stock introuvable");

    //    // 3. vérifier le stock
    //    if (stock.QuantiteDisponible < dto.QuantiteSortie)
    //        throw new Exception("Stock insuffisant");

    //    // 4. déduire du stock
    //    stock.QuantiteDisponible -= dto.QuantiteSortie;

    //    // 5. *** NOUVEAU : mettre à jour la quantité de la palette ***
    //    var palette = reception.Palette;
    //    palette.QuantiteDisponible -= dto.QuantiteSortie;
    //    if (palette.QuantiteDisponible == 0)
    //        palette.EtatPalette = "Vide"; // ou "Épuisée"

    //    // 6. déterminer SocieteId
    //    var societeId = stock.SocieteId != 0 ? stock.SocieteId : reception.SocieteId;
    //    if (societeId == 0)
    //        throw new Exception("SocieteId introuvable");

    //    // 7. créer la sortie
    //    var sortie = new SortieStock
    //    {
    //        ReceptionId = reception.Id,
    //        QuantiteSortie = dto.QuantiteSortie,
    //        DateSortie = DateTime.UtcNow,
    //        Utilisateur = dto.Utilisateur,
    //        Observation = dto.Observation,
    //        SocieteId = societeId
    //    };

    //    _context.SortieStocks.Add(sortie);
    //    await _context.SaveChangesAsync();

    //    return sortie;
    //}





    // logique métier pour créer une sortie de stock à partir d'une palette
    //public async Task<SortieStock> CreateSortieAsync(CreateSortieStockDto dto)
    //{
    //    // 1. chercher réception via code palette
    //    var reception = await _context.Receptions
    //        .Include(r => r.Palette)
    //        .FirstOrDefaultAsync(r =>
    //            r.Palette != null &&
    //            r.Palette.CodePalette == dto.CodePalette);

    //    if (reception == null)
    //        throw new Exception("Palette introuvable");

    //    // 2. récupérer stock produit
    //    var stock = await _context.Stocks
    //  .FirstOrDefaultAsync(s =>
    //      s.ProduitId == reception.Palette!.ProduitId);

    //    if (stock == null)
    //        throw new Exception("Stock introuvable");

    //    // 3. contrôle stock
    //    if (stock.QuantiteDisponible < dto.QuantiteSortie)
    //        throw new Exception("Stock insuffisant");

    //    // 4. déduction stock
    //    stock.QuantiteDisponible -= dto.QuantiteSortie;

    //    var societeId = stock?.SocieteId ?? reception.SocieteId;

    //    if (societeId == 0)
    //        throw new Exception("Impossible de déterminer SocieteId pour la sortie");


    //    // 5. créer sortie
    //    var sortie = new SortieStock
    //    {
    //        ReceptionId = reception.Id,
    //        QuantiteSortie = dto.QuantiteSortie,
    //        DateSortie = DateTime.UtcNow,
    //        Utilisateur = dto.Utilisateur,
    //        Observation = dto.Observation,
    //        SocieteId = societeId
    //    };

    //    _context.SortieStocks.Add(sortie);

    //    await _context.SaveChangesAsync();

    //    return sortie;
    //}

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