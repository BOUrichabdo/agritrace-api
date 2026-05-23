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
            Console.WriteLine("=== CREATE SOCIETE ===");
            Console.WriteLine($"Nom: {dto.Nom}");
            Console.WriteLine($"Email: {dto.Email}");
            Console.WriteLine($"AdminEmail: {dto.AdminEmail}");

            // Vérifier si l'email existe déjà
            var existingUser = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == dto.AdminEmail);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Cet email est déjà utilisé" });
            }

            // 1. Créer la société
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
                DateCreation = DateTime.UtcNow,
                IsActive = true
            };

            _context.Societes.Add(societe);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Société créée avec ID: {societe.Id}");

            // 2. Créer l'utilisateur admin
            var admin = new Utilisateur
            {
                Nom = dto.AdminNom,
                Email = dto.AdminEmail,
                //MotDePasse = BCrypt.Net.BCrypt.HashPassword(dto.AdminPassword), // Hash du mot de passe

                MotDePasse = dto.AdminPassword, // Hash du mot de passe

                Role = "Admin",
                SocieteId = societe.Id,
                DateCreation = DateTime.UtcNow,
                IsActive = true
            };

            _context.Utilisateurs.Add(admin);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Admin créé avec ID: {admin.Id}");

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
            Console.WriteLine($"❌ ERREUR: {ex.Message}");
            Console.WriteLine($"STACK: {ex.StackTrace}");

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