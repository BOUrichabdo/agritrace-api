using AgriTraceApp.DTOs;
using AgriTraceApp.Models;
using AgriTraceApp.Services;
using System.Globalization;

namespace AgriTraceApp;

public partial class Historique : ContentPage
{
    // liste produit 
    private List<ProduitModel> _produit = new();
    // produit services 
    private ProduitService _service;

    ReceptionResponseDto reception;
    // reception services 
    private readonly ReceptionService _receptionService;
    private List<ReceptionResponseDto> toutesLesReceptions;
    private List<ReceptionResponseDto>? receptionsFiltrees;
    // pagination 
    private int pageActuelle = 1;
    private const int ITEMS_PAR_PAGE = 8;
    private int totalPages = 1;
    public Historique()
    {
        InitializeComponent();

        _service = new ProduitService();
        _receptionService = new ReceptionService();
        toutesLesReceptions = new List<ReceptionResponseDto>();


        // Forcer le format de date français
        var culture = new System.Globalization.CultureInfo("fr-FR");
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

        // Pour .NET MAUI
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;




        // f,iltrage 

        FiltreDate.PropertyChanged += OnFiltreChanged;
        FiltreProduit.SelectedValueChanged += OnFiltreChanged;


    }

    // methode tital generale 
    //private void MettreAJourTotalGeneral()
    //{
    //    if (toutesLesReceptions != null && toutesLesReceptions.Any())
    //    {
    //        decimal totalGeneral = toutesLesReceptions.Sum(r => r.PoidsBrut);
    //        LblTotalKg.Text = $"{totalGeneral:N0} Kg";

    //        // Optionnel : Changer la couleur selon le montant
    //        if (totalGeneral > 10000)
    //            LblTotalKg.TextColor = Color.FromArgb("#10B981");
    //        else if (totalGeneral > 5000)
    //            LblTotalKg.TextColor = Color.FromArgb("#F59E0B");
    //        else
    //            LblTotalKg.TextColor = Color.FromArgb("#2563EB");
    //    }
    //    else
    //    {
    //        LblTotalKg.Text = "0 Kg";
    //    }
    //}


    // methode filtrage 
    private async void OnFiltreChanged(object? sender, object e)
    {
        await AppliquerFiltres();
    }

    // appliquer filtrage 
    //private async Task AppliquerFiltres()
    //{
    //    try
    //    {
    //        LoadingIndicator.IsVisible = true;
    //        LoadingIndicator.IsRunning = true;

    //        // Commencer avec toutes les réceptions
    //        var receptionsFiltrees = toutesLesReceptions.AsEnumerable();

    //        // Filtrage par date
    //        if (FiltreDate.Date.HasValue)
    //        {
    //            DateTime dateSelectionnee = FiltreDate.Date.Value.Date;
    //            receptionsFiltrees = receptionsFiltrees.Where(r => r.DateReception.Date == dateSelectionnee);
    //        }

    //        // Filtrage par produit
    //        if (FiltreProduit.SelectedIndex > 0) // Index 0 = "Tous"
    //        {
    //            string produitSelectionne = FiltreProduit.Items[FiltreProduit.SelectedIndex];
    //            receptionsFiltrees = receptionsFiltrees.Where(r => r.Produit == produitSelectionne);
    //        }

    //        // Mettre à jour la liste affichée
    //        ReceptionsList.ItemsSource = receptionsFiltrees.ToList();

    //        //// Afficher un message si aucun résultat
    //        //if (!receptionsFiltrees.Any())
    //        //{
    //        //    // Vous pouvez ajouter un Label d'information dans votre XAML
    //        //    await DisplayAlert("Information", "Aucune réception trouvée avec ces critères", "OK");
    //        //}
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Erreur", $"Erreur lors du filtrage: {ex.Message}", "OK");
    //    }
    //    finally
    //    {
    //        LoadingIndicator.IsVisible = false;
    //        LoadingIndicator.IsRunning = false;
    //    }
    //}

    private async Task AppliquerFiltres()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            if (toutesLesReceptions == null || !toutesLesReceptions.Any())
            {
                ReceptionsList.ItemsSource = null;
                LblTotalFiltreValue.Text = "0 Kg";
                return;
            }

            var receptionsFiltrees = toutesLesReceptions.AsEnumerable();

            // Filtrage par date
            if (FiltreDate.Date.HasValue)
            {
                DateTime dateSelectionnee = FiltreDate.Date.Value.Date;
                receptionsFiltrees = receptionsFiltrees.Where(r => r.DateReception.Date == dateSelectionnee);
            }

            // Filtrage par produit
            if (FiltreProduit.SelectedIndex > 0) // Index 0 = "Tous"
            {
                string produitSelectionne = FiltreProduit.Items[FiltreProduit.SelectedIndex];
                receptionsFiltrees = receptionsFiltrees.Where(r => r.Produit == produitSelectionne);
            }

            var listeFiltree = receptionsFiltrees.ToList();
            ReceptionsList.ItemsSource = listeFiltree;

            // Mettre à jour le total filtré
            MettreAJourTotalFiltre(listeFiltree);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors du filtrage: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }




    private void MettreAJourTotalFiltre(List<ReceptionResponseDto> receptionsFiltrees)
    {
        if (receptionsFiltrees != null && receptionsFiltrees.Any())
        {
            decimal totalFiltre = receptionsFiltrees.Sum(r => r.PoidsBrut);
            LblTotalFiltreValue.Text = $"{totalFiltre:N0} Kg";

            // Changer la couleur selon le montant
            if (totalFiltre > 10000)
                LblTotalFiltreValue.TextColor = Color.FromArgb("#10B981");
            else if (totalFiltre > 5000)
                LblTotalFiltreValue.TextColor = Color.FromArgb("#F59E0B");
            else
                LblTotalFiltreValue.TextColor = Color.FromArgb("#2563EB");

            // Afficher le pourcentage par rapport au total général
            if (toutesLesReceptions != null && toutesLesReceptions.Any())
            {
                decimal totalGeneral = toutesLesReceptions.Sum(r => r.PoidsBrut);
                if (totalGeneral > 0)
                {
                    double pourcentage = (double)(totalFiltre / totalGeneral) * 100;
                    // Optionnel: Ajouter un ToolTip ou afficher le pourcentage
                    //LblTotalFiltre.Text = $"Filtré ({pourcentage:F0}%) :";
                }
                else
                {
                    //LblTotalFiltre.Text = "Filtré :";
                }
            }
        }
        else
        {
            LblTotalFiltreValue.Text = "0 Kg";
            LblTotalFiltreValue.TextColor = Color.FromArgb("#EF4444");
            //LblTotalFiltre.Text = "Filtré :";
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!FiltreDate.Date.HasValue)
        {
            FiltreDate.Date = DateTime.Today;
        }
        //await TestApiConnection();
        // charger produit 
        await RemplirProduit();
        // chargere reception 
        await ChargerReceptions();
    }

    private async Task TestApiConnection()
    {
        try
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://agritrace-api-production.up.railway.app/api/Receptions");
            var content = await response.Content.ReadAsStringAsync();

            await DisplayAlert("Test API",
                $"Status: {response.StatusCode}\n\n" +
                $"Contenu: {content?.Substring(0, Math.Min(300, content?.Length ?? 0))}",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Test API Error", ex.Message, "OK");
        }
    }


    private async Task ChargerReceptions()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            toutesLesReceptions = await _receptionService.GetReceptions();

            //// Définir la culture française pour les dates
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");

            //// Si vous utilisez .NET MAUI, définir également pour l'application
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-FR");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fr-FR");


            await AppliquerFiltres();
            //ReceptionsList.ItemsSource = toutesLesReceptions;
            //AfficherPage();
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
    //private void MettreAJourPagination()
    //{
    //    if (toutesLesReceptions.Count == 0)
    //    {
    //        totalPages = 1;
    //        LblPageActuelle.Text = "Page 0";
    //        LblTotalPages.Text = "0";
    //        BtnPrecedent.IsEnabled = false;
    //        BtnSuivant.IsEnabled = false;
    //        return;
    //    }

    //    totalPages = (int)Math.Ceiling((double)toutesLesReceptions.Count / ITEMS_PAR_PAGE);

    //    LblPageActuelle.Text = $"Page {pageActuelle}";
    //    LblTotalPages.Text = totalPages.ToString();

    //    BtnPrecedent.IsEnabled = pageActuelle > 1;
    //    BtnSuivant.IsEnabled = pageActuelle < totalPages;
    //}

    private void AfficherPage()
    {
        //ReceptionsList.Children.Clear();

        if (!toutesLesReceptions.Any())
        {
            //LblEmptyMessage.IsVisible = true;
            return;
        }

        //LblEmptyMessage.IsVisible = false;

        //var itemsPage = toutesLesReceptions
        //    .Skip((pageActuelle - 1) * ITEMS_PAR_PAGE)
        //    .Take(ITEMS_PAR_PAGE)
        //    .ToList();

        //foreach (var reception in itemsPage)
        //{
        //    ReceptionsList.Children.Add(CreerLigneReception(reception));
        //}
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
        //if (pageActuelle > 1)
        //{
        //    pageActuelle--;
        //    MettreAJourPagination();
        //    AfficherPage();
        //}
    }

    private void OnSuivantClicked(object sender, EventArgs e)
    {
        //if (pageActuelle < totalPages)
        //{
        //    pageActuelle++;
        //    MettreAJourPagination();
        //    AfficherPage();
        //}
    }

    private void BtnPrecedent_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnSuivant_Clicked(object sender, EventArgs e)
    {

    }

    private void SwipeItem_Invoked(object sender, EventArgs e)
    {

    }

    private async void SwipeItem_Invoked_1(object sender, EventArgs e)
    {

        var swipeItem = sender as SwipeItem;

        if (swipeItem?.CommandParameter is ReceptionResponseDto reception)
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


    }

    private async void SwipeItem_Invoked_2(object sender, EventArgs e)
    {


        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        var swipeItem = sender as SwipeItem;

        if (swipeItem?.CommandParameter is ReceptionResponseDto reception)
        {


            bool confirmer = await DisplayAlert("Impression Réception",
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
                      "Confirmer",
                      "Annuler");



            if (!confirmer)
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                return;
            }

            try
            {





                if (reception.PaletteId <= 0)
                {
                    await DisplayAlert("Erreur", "Aucune palette à imprimer", "OK");
                    return;
                }

                var service = new ReceptionService();

                var pdfBytes = await service.PrintPalette(reception.PaletteId);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    await DisplayAlert("Erreur", "PDF vide ou introuvable", "OK");
                    return;
                }

                string fileName =
                    $"PAL_{reception.PaletteId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

                string filePath =
                    Path.Combine(FileSystem.CacheDirectory, fileName);

                File.WriteAllBytes(filePath, pdfBytes);

                await Launcher.Default.OpenAsync(
                    new OpenFileRequest
                    {
                        Title = "Palette PDF",
                        File = new ReadOnlyFile(filePath)
                    });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }








        }






















    }

    private void BtnRefresh_Clicked(object sender, EventArgs e)
    {

    }

    private async void BtnRefresh_Clicked_1(object sender, EventArgs e)
    {
        // Réinitialiser les filtres
        FiltreDate.Date = DateTime.Today;
        FiltreProduit.SelectedIndex = 0;

        // Recharger les données
        await ChargerReceptions();

    }

    //private void FiltreDate_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(DatePickerField.Date))
    //    {
    //        OnFiltreDateChanged();
    //    }
    //}

    //private void OnFiltreDateChanged()
    //{
    //    // Placez ici la logique de filtrage à appliquer lors du changement de date
    //    // Par exemple : rafraîchir la liste des réceptions filtrées
    //}
}