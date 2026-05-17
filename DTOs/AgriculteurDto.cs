using TracAgriApi.Models;

namespace TracAgriApi.DTOs
{
    public class AgriculteurDto
    {

        public int Id { get; set; }   // clé primaire
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;


        public string Nomferme { get; set; } = string.Empty;








    }
}
