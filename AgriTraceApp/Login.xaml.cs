using AgriTraceApp.DTOs;
using AgriTraceApp.Services;

namespace AgriTraceApp;

public partial class Login : ContentPage
{

    private bool _isPasswordVisible = false;

    public Login()
	{
		InitializeComponent();
	}

    private async void BTNCONNECTER_Clicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        try
        {
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

            // Stockage la session  0

            Preferences.Set("token", result.Token);
            Preferences.Set("role", result.Role);
            Preferences.Set("societeId", result.SocieteId);


            // Correction CS0618 et CS8602
            var app = Application.Current;
            if (app != null && app.Windows.Count > 0)
            {
                var mainWindow = app.Windows[0];
                if (result.SocieteId == 0)
                {
                    mainWindow.Page = new Societe();
                }
                else
                {
                    mainWindow.Page = new AppShell();
                }
            }
            else
            {

                await DisplayAlert("Erreur", "Impossible de trouver la fenêtre principale de l'application.", "OK");


            }
         
        }
        catch (Exception ex)
        {
            await DisplayAlert(
               "Erreur",
               ex.Message,
               "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void btnTogglePassword_Clicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        TXT_PASSWORD.IsPassword = !_isPasswordVisible;

        // Méthode 1 : avec le nom du fichier (recommandé)
        btnTogglePassword.Source = _isPasswordVisible ? "eye_open.png" : "eye_closed.png";

    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void BTN_CREER_SOCIETE_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new Societe());

    }

    private void btnTogglePassword_Clicked_1(object sender, EventArgs e)
    {

        _isPasswordVisible = !_isPasswordVisible;
        TXT_PASSWORD.IsPassword = !_isPasswordVisible;

        // Changer l'icône
        btnTogglePassword.Source = _isPasswordVisible ? "eye_open.png" : "eye_closed.png";

    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {

    }
}