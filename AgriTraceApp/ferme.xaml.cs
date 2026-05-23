using AgriTraceApp.Models;
using AgriTraceApp.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AgriTraceApp;

public partial class Ferme : ContentPage
{

    // varible pour gerer modification 
    private bool isEditMode = false;
    private int selectedId = 0;
    // instance service 
    private FermeService _servicesferme = new FermeService();
    private AgriculteurService _serviceafriculteur = new AgriculteurService();
    // objet Model agriculteur 
    private List<AgriculteurModel> ? _agriculteurs;
    private List<FermeModele> _fermes = new();
    private CancellationTokenSource _cts;
    public  Ferme()
	{
		InitializeComponent();

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();

      
    }
     // remplire liste agriculteur                                                               
    private async Task LoadAgriculteurs()
    {
        _agriculteurs = await _serviceafriculteur.GetAgriculteurs();

        CMBOAGRI.ItemsSource = _agriculteurs;
    }

    private async Task LoadData()
    {
        //// remplire liste fereme
        //var data = await _service.GetFermes();
        //ListeFerme.ItemsSource = data;
        //// remplire picker Agriculteur
        //await LoadAgriculteurs();


        // 🔥 stocker données dans la liste principale
        _fermes = await _servicesferme.GetFermes();

        // 🔥 afficher
        ListeFerme.ItemsSource = _fermes;

        // 🔥 charger agriculteurs
        await LoadAgriculteurs();

    }


    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    // btn Insertion et modification Ferme
    private async void VALIDER_Clicked(object sender, EventArgs e)
    {
        // controle de saisie 
        bool isValid = true;
        // RESET ERRORS
        FERMEERREUR.IsVisible = false;
        // combo agriculteur
      

        // 🔴 NOM
        if (string.IsNullOrWhiteSpace(FEREMEENTRY.Text))
        {
            FERMEERREUR.Text = "Nom obligatoire";
            FERMEERREUR.IsVisible = true;
            FEREMEENTRY.Focus();

            isValid = false;
        }
        var selectedAgri = CMBOAGRI.SelectedItem as AgriculteurModel;

        if (selectedAgri == null)
        {
            ERREURAGRI.Text = "Veuillez choisir un agriculteur";
            ERREURAGRI.IsVisible = true;
            CMBOAGRI.Focus();
            isValid = false;
        }
        // ❌ STOP SI ERREUR
        if (!isValid)
            return;

        // verification action 
        if (isEditMode)
        {
            // 🔄 UPDATE
            var agri = new FermeModele
            {
                Id = selectedId,
                NomFerme = FEREMEENTRY.Text,
                AgriculteurId = selectedAgri.Id
            };
            await _servicesferme.UpdateFerme(agri);
            await Snackbar.Make("Modifié avec succès ✏️").Show();
        }
        // insertion mode 
        else
        {
            try
            {
                var Ferme = new FermeModele
                {
                    NomFerme = FEREMEENTRY.Text.Trim(),
                    AgriculteurId = selectedAgri.Id

                };

                await _servicesferme.AddFerme(Ferme);
                var snackbar = Snackbar.Make("Ferme ajouté avec succès ✅", duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White
                    });

                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make("Erreur lors de l'ajout ❌", duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White
                    });

                await snackbar.Show();


            }

        }
        // Actualiser
        ClearForm();
        await LoadData();
    }
    // actualiser page
    private void ClearForm()
    {
        FEREMEENTRY.Text = "";

        isEditMode = false;
        selectedId = 0;

        CMBOAGRI.SelectedItem = null; // 🔥 reset picker


        VALIDER.Text = "Ajouter";
        VALIDER.BackgroundColor = Color.FromArgb("#2E7D32");

        ERREURAGRI.IsVisible = false;
        FERMEERREUR.IsVisible = false;
    }

    private void SearchEntry_TextChanged_1(object sender, TextChangedEventArgs e)
    {

    }

    private  async void SearchEntry_TextChanged_2(object sender, TextChangedEventArgs e)
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
                    ListeFerme.ItemsSource = _fermes;
                });

                return;
            }

            // 🔎 filtrage
            var result = _fermes
                .Where(f =>
                    !string.IsNullOrWhiteSpace(f.NomFerme) &&
                    f.NomFerme.Contains(keyword,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 🔥 update UI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListeFerme.ItemsSource = result;
            });
        }
        catch (TaskCanceledException)
        {

        }

    }

    private void UPDATE_Invoked(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (FermeModele)swipe.CommandParameter;

        // remplir champs
        FEREMEENTRY.Text = agri.NomFerme;



        // mode edit
        isEditMode = true;
        selectedId = agri.Id;

        int SelectIdagri = agri.AgriculteurId;


        CMBOAGRI.SelectedItem = _agriculteurs
    .FirstOrDefault(a => a.Id == SelectIdagri);

        // changer bouton UI
        VALIDER.Text = "Modifier";
        VALIDER.BackgroundColor = Colors.Orange;

    }

    private async void DELETE_Invoked(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (FermeModele)swipe.CommandParameter;

        bool confirm = await DisplayAlert(
            "Supprimer ?",
            $"Supprimer {agri.NomFerme} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _servicesferme.DeleteFerme(agri.Id);
        await LoadData();

        //// afficher infos supprimées dans formulaire (comme tu veux)
        //TXT_AGRICULTEUR.Text = agri.Nom;
        //TELE.Text = agri.Telephone;
        //ADRESSE.Text = agri.Adresse;

        await Snackbar.Make("Ferme supprimé 🗑").Show();

    }

    private  void SearchEntry_TextChanged_3(object sender, TextChangedEventArgs e)
    {


       
    }

    private void FEREMEENTRY_TextChanged(object sender, TextChangedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(FEREMEENTRY.Text))
            return;

        string text = FEREMEENTRY.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (FEREMEENTRY.Text != formatted)
        {
            FEREMEENTRY.Text = formatted;
        }
    }

}
