using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.DTOs
{
    public class EtiquetteDto
    {
        public int Id { get; set; }

        public string CodeEtiquette { get; set; } = string.Empty;

        public int AgriculteurId { get; set; }

        public string AgriculteurNom { get; set; } = string.Empty;

        public int FermeId { get; set; }

        public string FermeNom { get; set; } = string.Empty;

        public int ParcelleId { get; set; }

        public string ParcelleNom { get; set; } = string.Empty;

        public int ProduitId { get; set; }

        public string ProduitNom { get; set; } = string.Empty;

        public int VarieteId { get; set; }

        public string VarieteNom { get; set; } = string.Empty;

        public DateTime DateGeneration { get; set; }

        public bool Receptionne { get; set; }

        public DateTime? DateReception { get; set; }

    }
}
