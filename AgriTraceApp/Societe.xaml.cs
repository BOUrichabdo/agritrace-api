using AgriTraceApp.DTOs;
using AgriTraceApp.Services;



namespace AgriTraceApp;

public partial class Societe : ContentPage
{
	public Societe()
	{
		InitializeComponent();
	}

    private void BTNVALIDER_Clicked(object sender, EventArgs e)
    {

    }

    private async void BTNVALIDER_Clicked_1(object sender, EventArgs e)
    {


        try
        {
            // =========================
            // VALIDATION
            // =========================

            if (string.IsNullOrWhiteSpace(TXT_NOM.Text))
            {
                await DisplayAlert(
                    "Erreur",
                    "Nom société obligatoire",
                    "OK");

                return;
            }

            // =========================
            // DTO
            // =========================

            var dto = new CreateSocieteDto
            {
                Nom = TXT_NOM.Text,
                NomCommercial = TXT_NOM_COMMERCIAL.Text,
                MatriculeFiscal = TXT_MATRICULE.Text,
                ICE = TXT_ICE.Text,

                Adresse = TXT_ADRESSE.Text,
                Ville = TXT_VILLE.Text,
                Telephone = TXT_TELEPHONE.Text,
                Email = TXT_EMAIL.Text,

                Plan = CMB_PLAN.SelectedItem?.ToString() ?? "Free",

                Devise = CMB_DEVISE.SelectedItem?.ToString() ?? "MAD",

                // ADMIN
                AdminNom = TXT_ADMIN_NOM.Text,
                AdminEmail = TXT_ADMIN_EMAIL.Text,
                AdminPassword = TXT_ADMIN_PASSWORD.Text
            };

            // =========================
            // API
            // =========================

            var service = new SocieteService();

            var result = await service.CreateSociete(dto);




            await DisplayAlert("DEBUG API", result, "OK");

            // =========================
            // DEBUG RESULT API
            // =========================

            await DisplayAlert(
                "API RESULT",
                result,
                "OK");

            // =========================
            // REDIRECTION
            // =========================

            await Navigation.PushAsync(new Login());
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erreur",
                ex.ToString(),
                "OK");
        }

















        //try
        //{
        //    // =========================
        //    // VALIDATION
        //    // =========================

        //    if (string.IsNullOrWhiteSpace(TXT_NOM.Text))
        //    {
        //        await DisplayAlert(
        //            "Erreur",
        //            "Nom société obligatoire",
        //            "OK");

        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(TXT_ADMIN_NOM.Text))
        //    {
        //        await DisplayAlert(
        //            "Erreur",
        //            "Nom administrateur obligatoire",
        //            "OK");

        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(TXT_ADMIN_PASSWORD.Text))
        //    {
        //        await DisplayAlert(
        //            "Erreur",
        //            "Mot de passe obligatoire",
        //            "OK");

        //        return;
        //    }

        //    // =========================
        //    // DTO
        //    // =========================

        //    var dto = new CreateSocieteDto
        //    {
        //        Nom = TXT_NOM.Text,
        //        NomCommercial = TXT_NOM_COMMERCIAL.Text,
        //        MatriculeFiscal = TXT_MATRICULE.Text,
        //        ICE = TXT_ICE.Text,

        //        Adresse = TXT_ADRESSE.Text,
        //        Ville = TXT_VILLE.Text,
        //        Telephone = TXT_TELEPHONE.Text,
        //        Email = TXT_EMAIL.Text,

        //        Plan = CMB_PLAN.SelectedItem?.ToString() ?? "Free",

        //        Devise = CMB_DEVISE.SelectedItem?.ToString() ?? "MAD",

        //        // ADMIN
        //        AdminNom = TXT_ADMIN_NOM.Text,
        //        AdminEmail = TXT_ADMIN_EMAIL.Text,
        //        AdminPassword = TXT_ADMIN_PASSWORD.Text
        //    };

        //    // =========================
        //    // API
        //    // =========================

        //    var service = new SocieteService();

        //    var result = await service.CreateSociete(dto);


        //    await DisplayAlert(
        //        "API RESULT",
        //        result,
        //        "OK");

        //    // REDIRECTION LOGIN

        //    await Navigation.PushAsync(new Login());
        //    }
        //    else
        //    {
        //        await DisplayAlert(
        //            "Erreur",
        //            "Erreur création société",
        //            "OK");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    await DisplayAlert(
        //        "Erreur",
        //        ex.Message,
        //        "OK");
        //}






        // contole saisie

        //var dto = new CreateSocieteDto
        //{
        //    Nom = TXT_NOM.Text,
        //    NomCommercial = TXT_NOM_COMMERCIAL.Text,
        //    MatriculeFiscal = TXT_MATRICULE.Text,
        //    ICE = TXT_ICE.Text,
        //    Adresse = TXT_ADRESSE.Text,
        //    Ville = TXT_VILLE.Text,
        //    Telephone = TXT_TELEPHONE.Text,
        //    Email = TXT_EMAIL.Text,
        //    Plan = CMB_PLAN.SelectedItem?.ToString() ?? "Free",
        //    Devise = CMB_DEVISE.SelectedItem?.ToString() ?? "MAD"
        //};

        //var service = new SocieteService();

        //var result = await service.CreateSociete(dto);

        //if (result)
        //{
        //    await DisplayAlert("Succès", "Société créée avec succès", "OK");

        //    // aller login ou main
        //    await Navigation.PushAsync(new MainPage());
        //}
        //else
        //{
        //    await DisplayAlert("Erreur", "Erreur création société", "OK");
        //}

    }
}