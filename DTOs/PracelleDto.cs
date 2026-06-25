using TracAgriApi.Models;

namespace TracAgriApi.DTOs
{
    public class ParcelleDto
    {
        public int Id { get; set; }
        public string NomParcelle { get; set; } = string.Empty;
        public int FermeId { get; set; }
        public string NomFerme { get; set; } = string.Empty; 
    }
}
