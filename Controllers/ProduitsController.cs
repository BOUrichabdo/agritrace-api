using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduitsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProduitsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL 
        // api/Produits
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll()
        {
            var data = await _context.Produites
                .Include(p => p.Parcelle)
                .Include(p => p.Variete)
                    .ThenInclude(v => v.Categorie)
                .Select(p => new ProduitDto
                {
                    Id = p.Id,
                    Nom = p.Nom,

                    ParcelleId = p.ParcelleId,
                    ParcelleNom = p.Parcelle != null
                        ? p.Parcelle.NomParcelle
                        : "",

                    VarieteId = p.VarieteId,
                    VarieteNom = p.Variete != null
                        ? p.Variete.Intitule
                        : "",

                    CategorieNom = p.Variete != null &&
                                    p.Variete.Categorie != null
                        ? p.Variete.Categorie.Intitule
                        : ""
                })
                .ToListAsync();

            return Ok(data);
        }

        // =========================
        // GET BY ID
        // api/Produits/5
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<ProduitDto>> GetById(int id)
        {
            var p = await _context.Produites
                .Include(x => x.Parcelle)
                .Include(x => x.Variete)
                    .ThenInclude(v => v.Categorie)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound();

            var dto = new ProduitDto
            {
                Id = p.Id,
                Nom = p.Nom,

                ParcelleId = p.ParcelleId,
                ParcelleNom = p.Parcelle?.NomParcelle,

                VarieteId = p.VarieteId,
                VarieteNom = p.Variete?.Intitule,

                CategorieNom = p.Variete?.Categorie?.Intitule
            };

            return Ok(dto);
        }

        // =========================
        // CREATE
        // api/Produits
        // =========================
        [HttpPost]
        public async Task<ActionResult> Create(ProduitDto dto)
        {
            var produit = new Produit
            {
                Nom = dto.Nom,
                ParcelleId = dto.ParcelleId,
                VarieteId = dto.VarieteId
            };

            _context.Produites.Add(produit);

            await _context.SaveChangesAsync();

            return Ok(new ProduitDto
            {
                Id = produit.Id,
                Nom = produit.Nom,
                ParcelleId = produit.ParcelleId,
                VarieteId = produit.VarieteId
            });
        }

        // =========================
        // UPDATE
        // api/Produits/5
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProduitDto dto)
        {
            var produit = await _context.Produites.FindAsync(id);

            if (produit == null)
                return NotFound();

            produit.Nom = dto.Nom;
            produit.ParcelleId = dto.ParcelleId;
            produit.VarieteId = dto.VarieteId;

            await _context.SaveChangesAsync();

            return Ok(new ProduitDto
            {
                Id = produit.Id,
                Nom = produit.Nom,
                ParcelleId = produit.ParcelleId,
                VarieteId = produit.VarieteId
            });
        }

        // =========================
        // DELETE
        // api/Produits/5
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var produit = await _context.Produites.FindAsync(id);

            if (produit == null)
                return NotFound();

            _context.Produites.Remove(produit);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // liste produit par parcelle 

        [HttpGet("byparcelle/{parcelleid}")]
        public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduitByparcelle(int parcelleid)
        {
            var data = await _context.Produites
                .Where(f => f.ParcelleId == parcelleid)
                .Select(f => new ProduitDto
                {
                    Id = f.Id,
                    Nom = f.Nom,
                    ParcelleId = f.ParcelleId,
                    VarieteId = f.VarieteId

                })
                .ToListAsync();

            return Ok(data);
        }


        // liste variete by produit                        

        [HttpGet("byvariete/{varieteid}")]
        public async Task<ActionResult<VarieteDto>> GetProduitByvariete(int varieteid)
        {
            var data = await _context.Varietes
                .Include(v => v.Categorie)
                .Where(v => v.Id == varieteid)
                .Select(v => new VarieteDto
                {
                    Id = v.Id,
                    Intitule = v.Intitule,
                    CategorieId = v.CategorieId,
                    CategorieNom = v.Categorie != null
                        ? v.Categorie.Intitule
                        : ""
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return NotFound();

            return Ok(data);
        }


    }
}