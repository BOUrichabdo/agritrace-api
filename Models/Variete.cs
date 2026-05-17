namespace TracAgriApi.Models
{
    public class Variete
    {
        public int Id { get; set; }
        public string Intitule { get; set; } = string.Empty;

        public int CategorieId { get; set; }
        public Categorie? Categorie { get; set; }

        public int SocieteId { get; set; }

    }
}
