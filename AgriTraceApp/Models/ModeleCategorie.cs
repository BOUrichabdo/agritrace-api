using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Models
{
    public class ModeleCategorie
    {

        public int Id { get; set; }

        public string Intitule { get; set; }

        public List<ProduitModel>? Produits { get; set; }
    }
}
