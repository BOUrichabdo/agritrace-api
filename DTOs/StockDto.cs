namespace TracAgriApi.DTOs
{
    public class StockDto
    {
        public int Id { get; set; }

        public int ReceptionId { get; set; }

        public string Produit { get; set; } = string.Empty;

        public string Variete { get; set; } = string.Empty;

        public decimal QuantiteDisponible { get; set; }

        public DateTime DateEntree { get; set; }

        public string EtatStock { get; set; } = string.Empty;

        public string? Emplacement { get; set; }
    }
}
