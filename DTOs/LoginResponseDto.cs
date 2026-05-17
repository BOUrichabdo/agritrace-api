namespace TracAgriApi.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public string Nom { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public int SocieteId { get; set; }

        public string Societe { get; set; } = string.Empty;
    }
}
