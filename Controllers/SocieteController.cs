using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Create(CreateSocieteDto dto)
    {
        // =========================
        // CREATE SOCIETE
        // =========================

        var societe = new Societe
        {
            Nom = dto.Nom,
            NomCommercial = dto.NomCommercial,
            MatriculeFiscal = dto.MatriculeFiscal,
            ICE = dto.ICE,
            Adresse = dto.Adresse,
            Ville = dto.Ville,
            Telephone = dto.Telephone,
            Email = dto.Email,
            Plan = dto.Plan,
            Devise = dto.Devise,
            DateCreation = DateTime.Now,
            IsActive = true, 
            
        };

        _context.Societes.Add(societe);

        await _context.SaveChangesAsync();

        // =========================
        // CREATE ADMIN USER
        // =========================

        var admin = new Utilisateur
        {
            Nom = dto.AdminNom,

            Email = dto.AdminEmail,

            MotDePasse = dto.AdminPassword,

            Role = "Admin",

            SocieteId = societe.Id
        };

        _context.Utilisateurs.Add(admin);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Société créée avec succès"
        });
    }
}