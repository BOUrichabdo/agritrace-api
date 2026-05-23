using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.DTOs
{
    public class ReceptionResponseDto
    {

        public int ReceptionId { get; set; }
        public int PaletteId { get; set; }
        public string CodePalette { get; set; } = string.Empty;
        public decimal PoidsBrut { get; set; }
        public decimal QuantiteDisponible { get; set; }
        public DateTime DateReception { get; set; }

        // Champs pour l'affichage
        public string? Produit { get; set; }
        public string? Variete { get; set; }
        public string? Agriculteur { get; set; }
        public string? Ferme { get; set; }
        public decimal Temperature { get; set; }
        public string? Etat { get; set; }
        public string? Type { get; set; }
        public string? Observation { get; set; }
        public string? CodeQR { get; set; }
    }

}

