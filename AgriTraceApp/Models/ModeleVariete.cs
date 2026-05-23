using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Models
{
    public class ModeleVariete
    {
        public int Id { get; set; }

        public string Intitule { get; set; } = string.Empty;

        public int CategorieId { get; set; }

        public string CategorieNom { get; set; }
    }
}


