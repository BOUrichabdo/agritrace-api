// CreateSocieteDto.cs - Version avec valeurs par défaut
namespace TracAgriApi.DTOs
{
    public class CreateSocieteDto
    {
        public string Nom { get; set; } = string.Empty;
        public string NomCommercial { get; set; } = string.Empty;
        public string MatriculeFiscal { get; set; } = string.Empty;
        public string ICE { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Plan { get; set; } = "Free";
        public string Devise { get; set; } = "MAD";
        public string AdminNom { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;

        // Ajouter ces propriétés si nécessaire
        public bool IsActive { get; set; } = true;
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}