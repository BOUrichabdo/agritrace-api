namespace TracAgriApi.Models
{
    public class Agriculteur
    {
        public int Id { get; set; }   // clé primaire
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;


        public int SocieteId { get; set; }

        // liste fereme agriculteur
        public List<Ferme> Fermes { get; set; } = new();


    }
}
