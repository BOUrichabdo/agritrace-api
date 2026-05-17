namespace TracAgriApi.Models
{
    public class Produit
    {

        public int Id { get; set; }


        public string Nom { get; set; } = string.Empty;


        public int ParcelleId { get; set; }

        // liste parcelle produit²
        public Parcelle? Parcelle { get; set; }

        public int VarieteId { get; set; }
        // liste variete produit
        public int SocieteId { get; set; }


        public Variete? Variete { get; set; }
    }
}
