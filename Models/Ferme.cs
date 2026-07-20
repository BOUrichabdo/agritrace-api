namespace TracAgriApi.Models
{
    public class Ferme
    {

        public int Id { get; set;}

        public string NomFerme { get; set; } = string.Empty;

        public int AgriculteurId { get; set; }

        public int SocieteId { get; set; }

        // liste agriculteur ferme
        public Agriculteur? Agriculteur { get; set; }

        public List<Parcelle> Parcelles { get; set; } = new();


   

    }
}
