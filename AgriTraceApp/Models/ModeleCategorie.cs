namespace AgriTraceApp.Models
{
    public class ModeleCategorie
    {

        public int Id { get; set; }

        public string Intitule { get; set; }

        public List<ProduitModel>? Produits { get; set; }
    }
}
