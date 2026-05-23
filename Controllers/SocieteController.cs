using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

[Route("api/[controller]")]
[ApiController]
public class SocieteController : ControllerBase
{
    private readonly AppDbContext _context;

    public SocieteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSocieteDto dto)
    {
        try
        {
            // 1. Créer la société AVEC IsActive et DateCreation
            var societe = new Societe
            {
                Nom = dto.Nom,
                NomCommercial = dto.NomCommercial ?? dto.Nom,
                MatriculeFiscal = dto.MatriculeFiscal ?? "",
                ICE = dto.ICE ?? "",
                Adresse = dto.Adresse ?? "",
                Ville = dto.Ville ?? "",
                Telephone = dto.Telephone ?? "",
                Email = dto.Email ?? "",
                Plan = dto.Plan ?? "Free",
                Devise = dto.Devise ?? "MAD",
                IsActive = true,  // OBLIGATOIRE
                DateCreation = DateTime.UtcNow  // OBLIGATOIRE
            };

            _context.Societes.Add(societe);
            await _context.SaveChangesAsync();

            // 2. Créer l'admin AVEC IsActive et DateCreation
            var admin = new Utilisateur
            {
                Nom = dto.AdminNom,
                Email = dto.AdminEmail,
                MotDePasse = dto.AdminPassword,
                Role = "Admin",
                SocieteId = societe.Id,
                IsActive = true,  // OBLIGATOIRE
                DateCreation = DateTime.UtcNow  // OBLIGATOIRE
            };

            _context.Utilisateurs.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Société créée avec succès",
                societeId = societe.Id,
                userId = admin.Id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message,
                innerMessage = ex.InnerException?.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var societe = await _context.Societes
            .Include(s => s.Utilisateurs)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (societe == null)
            return NotFound(new { message = "Société non trouvée" });

        return Ok(societe);
    }
}