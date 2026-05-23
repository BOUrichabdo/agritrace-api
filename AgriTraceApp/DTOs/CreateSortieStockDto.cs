using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.DTOs
{
    public class CreateSortieStockDto
    {
        public string CodePalette { get; set; } = string.Empty;

        public decimal QuantiteSortie { get; set; }

        public string Utilisateur { get; set; } = string.Empty;

        public string Observation { get; set; } = string.Empty;
    }
}
