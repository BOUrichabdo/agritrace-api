using AgriTraceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TracAgriApi.DTOs;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(
            AppDbContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Utilisateurs
                .Include(u => u.Societe)
                .FirstOrDefaultAsync(u =>
                    u.Nom == dto.Nom &&
                    u.MotDePasse == dto.MotDePasse);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Email ou mot de passe incorrect"
                });
            }
            // le principe qui doit compri est creation 
            // claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Nom),

                new Claim(ClaimTypes.Email, user.Email),

                new Claim(ClaimTypes.Role, user.Role),

                new Claim("SocieteId",
                    user.SocieteId.ToString())
            };



            // key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["Jwt:Key"]!));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            // token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new LoginResponseDto
            {
                Token = jwt,

                Nom = user.Nom,

                Role = user.Role,

                SocieteId = user.SocieteId,

                Societe = user.Societe?.Nom ?? ""
            });
        }
    }
}