using AgriTraceApp.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.Metrics;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using AgriTraceApp.Services;

namespace AgriTraceApp;

public partial class Agriculteur 
{
    // varible pour gerer modification 
    private bool isEditMode = false;
    private int selectedId = 0;

    // filtrage 
    private List<AgriculteurModel> _agriculteur = new();
    private CancellationTokenSource? _cts;

    private AgriculteurService _service = new AgriculteurService();

    public Agriculteur()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
    }
    private async Task LoadData()
    {
        //var data = await _service.GetAgriculteurs();
        //AgriculteurList1.ItemsSource = data;


        // 🔥 stocker données dans la liste principale
        _agriculteur = await _service.GetAgriculteurs();

        // 🔥 afficher
        AgriculteurList1.ItemsSource = _agriculteur;


    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private void BTNMODIFIER_Clicked(object sender, EventArgs e)
    {

        DisplayAlert("Info", "Bouton modifier cliqué", "OK");


    }

    private void BTNDELETE_Clicked(object sender, EventArgs e)
    {

        DisplayAlert("Info", "Bouton Supp cliqué", "OK");


    }

    private async void BTNSUPP_Clicked(object sender, EventArgs e)
    {

        var button = sender as Button;
        var id = (int)button.CommandParameter;

        // 🔥 confirmation utilisateur
        bool confirm = await DisplayAlert(
            "Confirmation",
            "Voulez-vous supprimer cet agriculteur ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        try
        {
            await _service.DeleteAgriculteur(id);

            // snackbar succès
            var snackbar = Snackbar.Make(
                "Agriculteur supprimé avec succès ✅",
                duration: TimeSpan.FromSeconds(3),
                visualOptions: new SnackbarOptions
                {
                    BackgroundColor = Colors.Green,
                    TextColor = Colors.White
                });

            await snackbar.Show();

            await LoadData();
        }
        catch
        {
            var snackbar = Snackbar.Make(
              "Erreur suppression ❌",
              duration: TimeSpan.FromSeconds(3),
              visualOptions: new SnackbarOptions
              {
                  BackgroundColor = Colors.Green,
                  TextColor = Colors.White
              });

            await snackbar.Show();
        }

    }

    private void BTNMODIFIER_Clicked_1(object sender, EventArgs e)
    {

    }

    private async void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {

        // 🔥 annuler ancienne recherche
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            // debounce
            await Task.Delay(300, _cts.Token);

            string keyword = e.NewTextValue?.Trim() ?? "";

            // 🔄 afficher tout
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AgriculteurList1.ItemsSource = _agriculteur;
                });

                return;
            }

            // 🔎 filtrage
            var result = _agriculteur
                .Where(f =>
                    !string.IsNullOrWhiteSpace(f.Nom) &&
                    f.Nom.Contains(keyword,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 🔥 update UI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AgriculteurList1.ItemsSource = result;
            });
        }
        catch (TaskCanceledException)
        {

        }


    }
    // btn validation agriculteur 
    private async void VALIDER_Clicked(object sender, EventArgs e)
    {
        // controle de saisie 
        bool isValid = true;
        // RESET ERRORS
        NomError.IsVisible = false;
        TelError.IsVisible = false;
        AdrError.IsVisible = false;

        // 🔴 NOM
        if (string.IsNullOrWhiteSpace(TXT_AGRICULTEUR.Text))
        {
            NomError.Text = "Nom obligatoire";
            NomError.IsVisible = true;
            isValid = false;
        }

        // 🔴 TELEPHONE
        if (string.IsNullOrWhiteSpace(TELE.Text))
        {
            TelError.Text = "Téléphone obligatoire";
            TelError.IsVisible = true;
            isValid = false;
        }
        else if (TELE.Text.Length < 8)
        {
            TelError.Text = "Téléphone invalide";
            TelError.IsVisible = true;
            isValid = false;
        }

        // 🔴 ADRESSE
        if (string.IsNullOrWhiteSpace(ADRESSE.Text))
        {
            AdrError.Text = "Adresse obligatoire";
            AdrError.IsVisible = true;
            isValid = false;
        }

        // ❌ STOP SI ERREUR
        if (!isValid)
            return;

        // verification action 
        if (isEditMode)
        {
            // 🔄 UPDATE
            var agri = new AgriculteurModel
            {
                Id = selectedId,
                Nom = TXT_AGRICULTEUR.Text,
                Telephone = TELE.Text,
                Adresse = ADRESSE.Text
            };

            await _service.UpdateAgriculteur(agri);

            await Snackbar.Make("Modifié avec succès ✏️").Show();
        }
        else
        {
            try
            {
                var agriculteur = new AgriculteurModel
                {
                    Nom = TXT_AGRICULTEUR.Text.Trim(),
                    Telephone = TELE.Text.Trim(),
                    Adresse = ADRESSE.Text.Trim()
                };

                await _service.AddAgriculteur(agriculteur);

                var snackbar = Snackbar.Make(
                    "Agriculteur ajouté avec succès ✅",
                    duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White
                    });

                await snackbar.Show();
                //// reset form
                //TXT_AGRICULTEUR.Text = "";
                //TELE.Text = "";
                //ADRESSE.Text = "";

                //await LoadData();


            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make(
                    "Erreur lors de l'ajout ❌",
                    duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White
                    });

                await snackbar.Show();


            }

        }

        // reset
        ClearForm();
        await LoadData();





        


    }

    private void ClearForm()
    {
        TXT_AGRICULTEUR.Text = "";
        TELE.Text = "";
        ADRESSE.Text = "";

        isEditMode = false;
        selectedId = 0;

        VALIDER.Text = "Ajouter";
        VALIDER.BackgroundColor = Color.FromArgb("#2E7D32");
    }

    private async void SwipeItem_Invoked(object sender, EventArgs e)
    {

        var swipeItem = sender as SwipeItem;
        int id = (int)swipeItem.CommandParameter;

        bool confirm = await DisplayAlert(
            "Confirmation",
            "Supprimer cet agriculteur ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        try
        {
            await _service.DeleteAgriculteur(id);

            await LoadData();



            var snackbar = Snackbar.Make(
          "Supprimé avec succès ✅",
          duration: TimeSpan.FromSeconds(3),
          visualOptions: new SnackbarOptions
          {
              BackgroundColor = Colors.Red,
              TextColor = Colors.White
          });

            await snackbar.Show();
        }
        catch
        {
    

            var snackbar = Snackbar.Make(
   "Erreur suppression ❌",
   duration: TimeSpan.FromSeconds(3),
   visualOptions: new SnackbarOptions
   {
       BackgroundColor = Colors.Red,
       TextColor = Colors.White
   });

            await snackbar.Show();
        }

    }

    private void SwipeItem_Invoked_1(object sender, EventArgs e)
    {

    }

    private void SwipeItem_Invoked_2(object sender, EventArgs e)
    {

    }

    private void Gauche_Invoked(object sender, EventArgs e)
    {

    }

    private async void DELETE_Invoked(object sender, EventArgs e)
    {

        var item = sender as SwipeItem;
        int id = (int)item.CommandParameter;

        bool confirm = await DisplayAlert("Confirmation", "Supprimer ?", "Oui", "Non");

        if (!confirm) return;

        await _service.DeleteAgriculteur(id);
        await LoadData();

        await Snackbar.Make("Supprimé ✅").Show();

    }

    private async void UPDATE_Invoked(object sender, EventArgs e)
    {

        var item = sender as SwipeItem;
        int id = (int)item.CommandParameter;

        var agriculteur = (await _service.GetAgriculteurs())
                            .FirstOrDefault(x => x.Id == id);

        if (agriculteur == null)
            return;

        // exemple simple : remplir formulaire
        TXT_AGRICULTEUR.Text = agriculteur.Nom;
        TELE.Text = agriculteur.Telephone;
        ADRESSE.Text = agriculteur.Adresse;

        await Snackbar.Make("Mode modification ✏️").Show();
    }

    private void UPDATE_Invoked_1(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (AgriculteurModel)swipe.CommandParameter;

        // remplir champs
        TXT_AGRICULTEUR.Text = agri.Nom;
        TELE.Text = agri.Telephone;
        ADRESSE.Text = agri.Adresse;

        // mode edit
        isEditMode = true;
        selectedId = agri.Id;

        // changer bouton UI
        VALIDER.Text = "Modifier";
        VALIDER.BackgroundColor = Colors.Orange;

    }

    private async void DELETE_Invoked_1(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (AgriculteurModel)swipe.CommandParameter;

        bool confirm = await DisplayAlert(
            "Supprimer ?",
            $"Supprimer {agri.Nom} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _service.DeleteAgriculteur(agri.Id);
        await LoadData();

        //// afficher infos supprimées dans formulaire (comme tu veux)
        //TXT_AGRICULTEUR.Text = agri.Nom;
        //TELE.Text = agri.Telephone;
        //ADRESSE.Text = agri.Adresse;

        await Snackbar.Make("Agriculteur supprimé 🗑").Show();

    }

    private void VALIDER_Clicked_1(object sender, EventArgs e)
    {

    }

    private void SearchEntry_TextChanged_1(object sender, TextChangedEventArgs e)
    {

    }

    private void TXT_AGRICULTEUR_TextChanged(object sender, TextChangedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(TXT_AGRICULTEUR.Text))
            return;

        string text = TXT_AGRICULTEUR.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (TXT_AGRICULTEUR.Text != formatted)
        {
            TXT_AGRICULTEUR.Text = formatted;
        }

    }

    private void ADRESSE_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ADRESSE.Text))
            return;

        string text = ADRESSE.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (ADRESSE.Text != formatted)
        {
            ADRESSE.Text = formatted;
        }


    }
}