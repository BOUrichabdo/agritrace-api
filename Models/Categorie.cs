namespace TracAgriApi.Models
{
    public class Categorie
    {

        public int Id { get; set; }
        public string Intitule { get; set; } = string.Empty;


        public int SocieteId { get; set; }
        public List<Variete> Varietes { get; set; } = new();
    }
}
