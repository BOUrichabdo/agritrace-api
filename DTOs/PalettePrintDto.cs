namespace TracAgriApi.DTOs
{
    public class PalettePrintDto
    {
        public int Id { get; set; }
        public string CodePalette { get; set; } = string.Empty;

        public string AgriculteurNom { get; set; } = string.Empty;
        public string FermeNom { get; set; } = string.Empty;
        public string ProduitNom { get; set; } = string.Empty;
        public string VarieteNom { get; set; } = string.Empty;

        public decimal PoidsBrut { get; set; }
        public DateTime DateCreation { get; set; }

        public byte[] QrCode { get; set; } = [];
    }
}
