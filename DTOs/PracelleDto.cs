using TracAgriApi.Models;

namespace TracAgriApi.DTOs
{
    public class PracelleDto
    {

        public int Id { get; set; }
        public string NomParcelle { get; set; } = string.Empty;
        public int FermeId { get; set; }
        // liste parcelle ferme

        public int SocieteId { get; set; }

        public string Ferme { get; set; } = string.Empty;
    }
}
