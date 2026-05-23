using AgriTraceApp.DTOs;
using AgriTraceApp.Services;

namespace AgriTraceApp;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private async void BTNCONNECTER_Clicked(object sender, EventArgs e)
    {
        // controle saisie

        var api = new LoginServices();

        var dto = new LoginDto
        {
            Nom = TXT_USER.Text,
            MotDePasse = TXT_PASSWORD.Text
        };

        var result = await api.Login(dto);

        if (result == null)
        {
            await DisplayAlert("Erreur", "Login ou mot de passe incorrect", "OK");
            return;
        }

        // ✅ STOCKER SESSION
        Preferences.Set("token", result.Token);
        Preferences.Set("role", result.Role);
        Preferences.Set("societeId", result.SocieteId);


        if (result.SocieteId == 0)
        {
            Application.Current.MainPage = new Societe();
        }
        else
        {
            Application.Current.MainPage = new AppShell();
        }

        // ✅ NAVIGATE HOME



    }
}