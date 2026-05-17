using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Controllers;  



[Route("api/[controller]")]
[ApiController]
public class ParcelleController : ControllerBase
{

    private readonly AppDbContext ? _context;

    public ParcelleController(AppDbContext context)
    {
        _context  = context;
    }

    // GET
    [HttpGet]
    public async Task<ActionResult<List<Parcelle>>> Get()
    {
        return await _context.Parcelles
            .Include(p => p.Ferme)
            .ToListAsync();
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Add(Parcelle p)
    {
        if (string.IsNullOrWhiteSpace(p.NomParcelle))
            return BadRequest("Nom obligatoire");

        _context.Parcelles.Add(p);

        await _context.SaveChangesAsync();

        return Ok(p);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _context.Parcelles.FindAsync(id);

        if (p == null)
            return NotFound();

        _context.Parcelles.Remove(p);

        await _context.SaveChangesAsync();

        return Ok();
    }

    // PUT
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Parcelle p)
    {
        var existing =
            await _context.Parcelles.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.NomParcelle = p.NomParcelle;
        existing.FermeId = p.FermeId;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // get parcelle par id fereme 

    [HttpGet("byparcelle/{feremeid}")]
    public async Task<ActionResult<IEnumerable<Parcelle>>> GetParcellesByFerme(int feremeid)
    {
        var data = await _context.Parcelles
            .Where(f => f.FermeId == feremeid)
            .Select(f => new Parcelle
            {
                Id = f.Id,
                NomParcelle = f.NomParcelle,
                FermeId = f.FermeId
            })
            .ToListAsync();

        return Ok(data);
    }





}
