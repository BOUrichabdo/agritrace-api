namespace TracAgriApi.Models
{
    public class Societe
    {
        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;

        public string NomCommercial { get; set; } = string.Empty;

        public string MatriculeFiscal { get; set; } = string.Empty;

        public string ICE { get; set; } = string.Empty;

        public string Adresse { get; set; } = string.Empty;

        public string Ville { get; set; } = string.Empty;

        public string Telephone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // SaaS
        public string Plan { get; set; } = "Free";

        public string Devise { get; set; } = "MAD";

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public int SocieteId { get; set; }


        public List<Utilisateur>? Utilisateurs { get; set; }
    }
}
