public class Societe
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string NomCommercial { get; set; } = string.Empty;
    public string MatriculeFiscal { get; set; } = string.Empty;
    public string ICE { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Plan { get; set; } = "Free";
    public string Devise { get; set; } = "MAD";
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public List<Utilisateur>? Utilisateurs { get; set; }
}