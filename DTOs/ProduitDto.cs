namespace TracAgriApi.DTOs
{
    public class ProduitDto
    {

        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;

        public int ParcelleId { get; set; }
        public string? ParcelleNom { get; set; }

        public int VarieteId { get; set; }
        public string? VarieteNom { get; set; }

        public string? CategorieNom { get; set; }
    }
}
