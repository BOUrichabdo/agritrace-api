using System.Net.Sockets;

namespace TracAgriApi.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MotDePasse { get; set; } = string.Empty;

        public string Role { get; set; } = "Reception";

        // société
        public int SocieteId { get; set; }

        public Societe? Societe { get; set; }
    }
}
