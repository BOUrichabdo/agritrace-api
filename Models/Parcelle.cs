namespace TracAgriApi.Models
{
    public class Parcelle
    {
        public int Id { get; set; }
        public string NomParcelle { get; set; } = string.Empty;
        public int FermeId { get; set; }
        // liste parcelle ferme

        public int SocieteId { get; set; }

        public Ferme? Ferme { get; set; }
    }
}
