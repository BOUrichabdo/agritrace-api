namespace TracAgriApi.DTOs
{
    public class CreateStockDto
    {
        public int ReceptionId { get; set; }

        public int ProduitId { get; set; }

        public int VarieteId { get; set; }

        public decimal QuantiteDisponible { get; set; }

        public string? Emplacement { get; set; }
    }
}
