namespace TracAgriApi.DTOs
{
    public class ReceptionResponseDto
    {

        public int ReceptionId { get; set; }

        public int PaletteId { get; set; }


        public string CodePalette { get; set; } = string.Empty;

        public decimal PoidsBrut { get; set; }

        public decimal QuantiteDisponible { get; set; }

        public DateTime DateReception { get; set; }

        public string Produit { get; set; } = string.Empty;

        public string Agriculteur { get; set; } = string.Empty;
    }
}
