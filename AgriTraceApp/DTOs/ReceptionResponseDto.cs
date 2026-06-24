using System.Text.Json.Serialization;

namespace AgriTraceApp.DTOs
{
    public class ReceptionResponseDto
    {
        [JsonPropertyName("receptionId")]
        public int ReceptionId { get; set; }

        [JsonPropertyName("paletteId")]
        public int PaletteId { get; set; }

        [JsonPropertyName("codePalette")]
        public string CodePalette { get; set; } = string.Empty;

        [JsonPropertyName("poidsBrut")]
        public decimal PoidsBrut { get; set; }

        [JsonPropertyName("quantiteDisponible")]
        public decimal QuantiteDisponible { get; set; }

        [JsonPropertyName("dateReception")]
        public DateTime DateReception { get; set; }

        [JsonPropertyName("temperature")]
        public decimal Temperature { get; set; }

        [JsonPropertyName("etat")]
        public string Etat { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("observation")]
        public string Observation { get; set; } = string.Empty;

        [JsonPropertyName("codeQR")]
        public string CodeQR { get; set; } = string.Empty;

        [JsonPropertyName("agriculteur")]
        public string Agriculteur { get; set; } = string.Empty;

        [JsonPropertyName("ferme")]
        public string Ferme { get; set; } = string.Empty;

        [JsonPropertyName("produit")]
        public string Produit { get; set; } = string.Empty;

        [JsonPropertyName("variete")]
        public string Variete { get; set; } = string.Empty;
    }
}