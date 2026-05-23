using AgriTraceApp.DTOs;
using AgriTraceApp.Models;
using AgriTraceApp.Services;

namespace AgriTraceApp;

public partial class Historique : ContentPage
{

    private List<ProduitModel> _produit = new();
    private ProduitService _service;

    private readonly ReceptionService _receptionService;
    private List<ReceptionResponseDto> toutesLesReceptions;
    private List<ReceptionResponseDto> receptionsFiltrees;

    private int pageActuelle = 1;
    private const int ITEMS_PAR_PAGE = 8;
    private int totalPages = 1;

    public Historique()
	{
		InitializeComponent();

        _service = new ProduitService();
        _receptionService = new ReceptionService();
        toutesLesReceptions = new List<ReceptionResponseDto>();

        // Attacher les événements
        BtnPrecedent.Clicked += OnPrecedentClicked;
        BtnSuivant.Clicked += OnSuivantClicked;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RemplirProduit();
        await ChargerReceptions();



    }


    private async Task ChargerReceptions()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            toutesLesReceptions = await _receptionService.GetReceptions();

            MettreAJourStatistiques();
            MettreAJourPagination();
            AfficherPage();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les réceptions: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void MettreAJourStatistiques()
    {
        decimal totalKg = toutesLesReceptions.Sum(r => r.PoidsBrut);
        //LblTotalKg.Text = totalKg.ToString("N0");
    }


    // remplire liset des produit 
    private async Task RemplirProduit()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            // Récupérer les produits
            _produit = await _service.Getproduit();

            // Vider et remplir le PickerField
            FiltreProduit.Items.Clear();

            // Ajouter l'option "Tous" par défaut
            FiltreProduit.Items.Add("Tous");

            // Ajouter les produits
            if (_produit != null && _produit.Any())
            {
                foreach (var produit in _produit)
                {
                    // Supposons que votre objet Produit a une propriété Nom
                    FiltreProduit.Items.Add(produit.Nom);
                }
            }

            // Sélectionner "Tous" par défaut
            FiltreProduit.SelectedIndex = 0;

            //// Ajouter l'événement si pas déjà fait
            //FiltreProduit.SelectedIndexChanged -= OnFiltreProduitChanged; // Éviter les doublons
            //FiltreProduit.SelectedIndexChanged += OnFiltreProduitChanged;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les produits: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
    private void MettreAJourPagination()
    {
        if (toutesLesReceptions.Count == 0)
        {
            totalPages = 1;
            LblPageActuelle.Text = "Page 0";
            LblTotalPages.Text = "0";
            BtnPrecedent.IsEnabled = false;
            BtnSuivant.IsEnabled = false;
            return;
        }

        totalPages = (int)Math.Ceiling((double)toutesLesReceptions.Count / ITEMS_PAR_PAGE);

        LblPageActuelle.Text = $"Page {pageActuelle}";
        LblTotalPages.Text = totalPages.ToString();

        BtnPrecedent.IsEnabled = pageActuelle > 1;
        BtnSuivant.IsEnabled = pageActuelle < totalPages;
    }

    private void AfficherPage()
    {
        ReceptionsList.Children.Clear();

        if (!toutesLesReceptions.Any())
        {
            LblEmptyMessage.IsVisible = true;
            return;
        }

        LblEmptyMessage.IsVisible = false;

        var itemsPage = toutesLesReceptions
            .Skip((pageActuelle - 1) * ITEMS_PAR_PAGE)
            .Take(ITEMS_PAR_PAGE)
            .ToList();

        foreach (var reception in itemsPage)
        {
            ReceptionsList.Children.Add(CreerLigneReception(reception));
        }
    }

    private Grid CreerLigneReception(ReceptionResponseDto reception)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.6, GridUnitType.Star) }
                },
            Padding = new Thickness(12, 12),
            BackgroundColor = Color.FromArgb("#FFFFFF"),
            Margin = new Thickness(0, 1, 0, 0)
        };

        // Code QR
        string codeDisplay = !string.IsNullOrEmpty(reception.CodeQR)
            ? reception.CodeQR
            : reception.CodePalette;

        grid.Add(new Label
        {
            Text = codeDisplay?.Length > 12 ? codeDisplay.Substring(0, 12) + "..." : codeDisplay ?? "-",
            FontSize = 12,
            TextColor = Color.FromArgb("#374151")
        }, 0, 0);

        // Agriculteur
        grid.Add(new Label
        {
            Text = reception.Agriculteur ?? "-",
            FontSize = 12,
            TextColor = Color.FromArgb("#374151"),
            FontAttributes = FontAttributes.Bold
        }, 1, 0);

        // Produit + Variété
        var produitStack = new VerticalStackLayout { Spacing = 2 };
        produitStack.Children.Add(new Label
        {
            Text = reception.Produit ?? "-",
            FontSize = 12,
            TextColor = Color.FromArgb("#374151")
        });
        produitStack.Children.Add(new Label
        {
            Text = reception.Variete ?? "",
            FontSize = 10,
            TextColor = Color.FromArgb("#6B7280")
        });
        grid.Add(produitStack, 2, 0);

        // Poids
        grid.Add(new Label
        {
            Text = $"{reception.PoidsBrut:N0} kg",
            FontSize = 12,
            TextColor = Color.FromArgb("#374151")
        }, 3, 0);

        // État avec badge coloré
        var couleurEtat = reception.Etat == "Bon" ? Color.FromArgb("#16A34A") :
                          reception.Etat == "Moyen" ? Color.FromArgb("#F59E0B") :
                          reception.Etat == "Mauvais" ? Color.FromArgb("#EF4444") : Color.FromArgb("#9CA3AF");

        grid.Add(new Label
        {
            Text = reception.Etat ?? "-",
            FontSize = 12,
            TextColor = couleurEtat,
            FontAttributes = FontAttributes.Bold
        }, 4, 0);

        // Boutons d'action
        var actionsLayout = new HorizontalStackLayout
        {
            Spacing = 8,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var btnDetail = new Button
        {
            Text = "👁️",
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#2196F3"),
            FontSize = 16,
            WidthRequest = 35,
            HeightRequest = 35
        };
        btnDetail.Clicked += (s, e) => AfficherDetail(reception);

        var btnImprimer = new Button
        {
            Text = "🖨️",
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#F59E0B"),
            FontSize = 16,
            WidthRequest = 35,
            HeightRequest = 35
        };
        btnImprimer.Clicked += (s, e) => ImprimerEtiquette(reception);

        actionsLayout.Children.Add(btnDetail);
        actionsLayout.Children.Add(btnImprimer);
        grid.Add(actionsLayout, 5, 0);

        return grid;
    }

    private async void AfficherDetail(ReceptionResponseDto reception)
    {
        await DisplayAlert("Détail Réception",
            $"📦 Code QR: {reception.CodeQR ?? reception.CodePalette}\n\n" +
            $"👨‍🌾 Agriculteur: {reception.Agriculteur ?? "-"}\n" +
            $"🏡 Ferme: {reception.Ferme ?? "-"}\n\n" +
            $"🌱 Produit: {reception.Produit ?? "-"} ({reception.Variete ?? "-"})\n" +
            $"⚖️ Poids: {reception.PoidsBrut:N0} kg\n" +
            $"🌡️ Température: {reception.Temperature}°C\n" +
            $"📊 État: {reception.Etat ?? "-"}\n" +
            $"📁 Type: {reception.Type ?? "-"}\n" +
            $"📅 Date: {reception.DateReception:dd/MM/yyyy}\n\n" +
            $"📝 Observation: {reception.Observation ?? "Aucune"}",
            "OK");
    }

    private async void ImprimerEtiquette(ReceptionResponseDto reception)
    {
        await DisplayAlert("Impression",
            $"🖨️ Impression de l'étiquette\n\n" +
            $"Produit: {reception.Produit}\n" +
            $"Code: {reception.CodePalette}\n" +
            $"Poids: {reception.PoidsBrut} kg",
            "OK");
    }

    private void OnPrecedentClicked(object sender, EventArgs e)
    {
        if (pageActuelle > 1)
        {
            pageActuelle--;
            MettreAJourPagination();
            AfficherPage();
        }
    }

    private void OnSuivantClicked(object sender, EventArgs e)
    {
        if (pageActuelle < totalPages)
        {
            pageActuelle++;
            MettreAJourPagination();
            AfficherPage();
        }
    }

    private void BtnPrecedent_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnSuivant_Clicked(object sender, EventArgs e)
    {

    }
}