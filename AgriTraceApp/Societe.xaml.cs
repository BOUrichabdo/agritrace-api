using AgriTraceApp.DTOs;
using AgriTraceApp.Services;
using System.Text.RegularExpressions;

namespace AgriTraceApp;

public partial class Societe : ContentPage
{

    private bool _isCreating = false;

    public Societe()
    {
        InitializeComponent();
    }

    private void OnInputChanged(object sender, EventArgs e)
    {
        if (sender == TXT_NOM) ClearFieldError(BORDER_NOM, LBL_ERROR_NOM);
        else if (sender == TXT_NOM_COMMERCIAL) ClearFieldError(BORDER_NOM_COMMERCIAL, LBL_ERROR_NOM_COMMERCIAL);
        else if (sender == TXT_ICE) ClearFieldError(BORDER_ICE, LBL_ERROR_ICE);
        else if (sender == TXT_MATRICULE) ClearFieldError(BORDER_MATRICULE, LBL_ERROR_MATRICULE);
        else if (sender == TXT_ADRESSE) ClearFieldError(BORDER_ADRESSE, LBL_ERROR_ADRESSE);
        else if (sender == TXT_VILLE) ClearFieldError(BORDER_VILLE, LBL_ERROR_VILLE);
        else if (sender == TXT_TELEPHONE) ClearFieldError(BORDER_TELEPHONE, LBL_ERROR_TELEPHONE);
        else if (sender == TXT_EMAIL) ClearFieldError(BORDER_EMAIL, LBL_ERROR_EMAIL);
        else if (sender == CMB_PLAN) ClearFieldError(BORDER_PLAN, LBL_ERROR_PLAN);
        else if (sender == CMB_DEVISE) ClearFieldError(BORDER_DEVISE, LBL_ERROR_DEVISE);
        else if (sender == TXT_ADMIN_NOM) ClearFieldError(BORDER_ADMIN_NOM, LBL_ERROR_ADMIN_NOM);
        else if (sender == TXT_ADMIN_EMAIL) ClearFieldError(BORDER_ADMIN_EMAIL, LBL_ERROR_ADMIN_EMAIL);
        else if (sender == TXT_ADMIN_PASSWORD) ClearFieldError(BORDER_ADMIN_PASSWORD, LBL_ERROR_ADMIN_PASSWORD);
        else if (sender == TXT_ADMIN_CONFIRMATION) ClearFieldError(BORDER_ADMIN_CONFIRMATION, LBL_ERROR_ADMIN_CONFIRMATION);
    }

    private void SetFieldError(Border border, Label label, string message)
    {
        border.Stroke = new SolidColorBrush(Color.FromArgb("#EF4444"));
        border.StrokeThickness = 1.5;
        label.Text = message;
        label.IsVisible = true;
    }



    private void ClearFieldError(Border border, Label label)
    {
        border.Stroke = new SolidColorBrush(Color.FromArgb("#E5E7EB"));
        border.StrokeThickness = 1;
        label.IsVisible = false;
    }

    private bool ValidateForm()
    {
        bool isValid = true;
        // Email regex
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        // Phone regex (numbers, space, +, -, parenthese, length between 8 and 18)
        var phoneRegex = new Regex(@"^\+?[0-9\s\-()]{8,18}$");
        // ICE regex (exactly 15 digits)
        var iceRegex = new Regex(@"^\d{15}$");

        // 1. Nom de la société (Required)
        if (string.IsNullOrWhiteSpace(TXT_NOM.Text))
        {
            SetFieldError(BORDER_NOM, LBL_ERROR_NOM, "Le nom de la société est obligatoire.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_NOM, LBL_ERROR_NOM);
        }

        // 2. ICE (Optional, but if set must be 15 digits)
        if (!string.IsNullOrWhiteSpace(TXT_ICE.Text) && !iceRegex.IsMatch(TXT_ICE.Text.Trim()))
        {
            SetFieldError(BORDER_ICE, LBL_ERROR_ICE, "L'ICE doit être composé d'exactement 15 chiffres.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_ICE, LBL_ERROR_ICE);
        }

        // 3. Email entreprise (Optional, but if set must be valid)
        if (!string.IsNullOrWhiteSpace(TXT_EMAIL.Text) && !emailRegex.IsMatch(TXT_EMAIL.Text.Trim()))
        {
            SetFieldError(BORDER_EMAIL, LBL_ERROR_EMAIL, "L'adresse email n'est pas valide.");
            SetFieldError(BORDER_EMAIL, LBL_ERROR_EMAIL, "L'adresse email nd'est pas valide.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_EMAIL, LBL_ERROR_EMAIL);
        }

        // 4. Téléphone entreprise (Optional, but if set must be valid)
        if (!string.IsNullOrWhiteSpace(TXT_TELEPHONE.Text) && !phoneRegex.IsMatch(TXT_TELEPHONE.Text.Trim()))
        {
            SetFieldError(BORDER_TELEPHONE, LBL_ERROR_TELEPHONE, "Le numéro de téléphone n'est pas valide.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_TELEPHONE, LBL_ERROR_TELEPHONE);
        }

        // 5. Plan SaaS (Required)
        if (CMB_PLAN.SelectedItem == null)
        {
            SetFieldError(BORDER_PLAN, LBL_ERROR_PLAN, "Veuillez sélectionner un plan SaaS.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_PLAN, LBL_ERROR_PLAN);
        }

        // 6. Devise (Required)
        if (CMB_DEVISE.SelectedItem == null)
        {
            SetFieldError(BORDER_DEVISE, LBL_ERROR_DEVISE, "Veuillez sélectionner une devise.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_DEVISE, LBL_ERROR_DEVISE);
        }

        // 7. Admin Nom (Required)
        if (string.IsNullOrWhiteSpace(TXT_ADMIN_NOM.Text))
        {
            SetFieldError(BORDER_ADMIN_NOM, LBL_ERROR_ADMIN_NOM, "Le nom d'utilisateur administrateur est obligatoire.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_ADMIN_NOM, LBL_ERROR_ADMIN_NOM);
        }

        // 8. Admin Email (Required, must be valid)
        if (string.IsNullOrWhiteSpace(TXT_ADMIN_EMAIL.Text))
        {
            SetFieldError(BORDER_ADMIN_EMAIL, LBL_ERROR_ADMIN_EMAIL, "L'email administrateur est obligatoire.");
            isValid = false;
        }
        else if (!emailRegex.IsMatch(TXT_ADMIN_EMAIL.Text.Trim()))
        {
            SetFieldError(BORDER_ADMIN_EMAIL, LBL_ERROR_ADMIN_EMAIL, "L'email administrateur n'est pas valide.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_ADMIN_EMAIL, LBL_ERROR_ADMIN_EMAIL);
        }

        // 9. Admin Password (Required, min 6 chars)
        if (string.IsNullOrWhiteSpace(TXT_ADMIN_PASSWORD.Text))
        {
            SetFieldError(BORDER_ADMIN_PASSWORD, LBL_ERROR_ADMIN_PASSWORD, "Le mot de passe administrateur est obligatoire.");
            isValid = false;
        }
        else if (TXT_ADMIN_PASSWORD.Text.Length < 6)
        {
            SetFieldError(BORDER_ADMIN_PASSWORD, LBL_ERROR_ADMIN_PASSWORD, "Le mot de passe doit comporter au moins 6 caractères.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_ADMIN_PASSWORD, LBL_ERROR_ADMIN_PASSWORD);
        }

        // 10. Admin Password Confirmation (Required, must match)
        if (string.IsNullOrWhiteSpace(TXT_ADMIN_CONFIRMATION.Text))
        {
            SetFieldError(BORDER_ADMIN_CONFIRMATION, LBL_ERROR_ADMIN_CONFIRMATION, "Veuillez confirmer le mot de passe.");
            isValid = false;
        }
        else if (TXT_ADMIN_CONFIRMATION.Text != TXT_ADMIN_PASSWORD.Text)
        {
            SetFieldError(BORDER_ADMIN_CONFIRMATION, LBL_ERROR_ADMIN_CONFIRMATION, "Les mots de passe ne correspondent pas.");
            isValid = false;
        }
        else
        {
            ClearFieldError(BORDER_ADMIN_CONFIRMATION, LBL_ERROR_ADMIN_CONFIRMATION);
        }

        return isValid;
    }

    private async void BTNVALIDER_Clicked_1(object sender, EventArgs e)
    {
        // Évite les clics multiples pendant le traitement
        if (_isCreating) return;

        try
        {
            _isCreating = true;
            BTNVALIDER.IsEnabled = false;
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true; 

            // Validation du formulaire
            if (!ValidateForm())
            {
                await DisplayAlert("Erreur de saisie", "Veuillez corriger les erreurs signalées dans le formulaire.", "OK");
                return;
            }

            // Création du DTO
            var dto = new CreateSocieteDto
            {
                Nom = TXT_NOM.Text?.Trim() ?? string.Empty,
                NomCommercial = TXT_NOM_COMMERCIAL.Text?.Trim() ?? string.Empty,
                MatriculeFiscal = TXT_MATRICULE.Text?.Trim() ?? string.Empty,
                ICE = TXT_ICE.Text?.Trim() ?? string.Empty,
                Adresse = TXT_ADRESSE.Text?.Trim() ?? string.Empty,
                Ville = TXT_VILLE.Text?.Trim() ?? string.Empty,
                Telephone = TXT_TELEPHONE.Text?.Trim() ?? string.Empty,
                Email = TXT_EMAIL.Text?.Trim() ?? string.Empty,
                Plan = CMB_PLAN.SelectedItem?.ToString() ?? "Free",
                Devise = CMB_DEVISE.SelectedItem?.ToString() ?? "MAD",
                AdminNom = TXT_ADMIN_NOM.Text?.Trim() ?? string.Empty,
                AdminEmail = TXT_ADMIN_EMAIL.Text?.Trim() ?? string.Empty,
                AdminPassword = TXT_ADMIN_PASSWORD.Text ?? string.Empty
            };

            // Appel API
            var service = new SocieteService();
            var result = await service.CreateSociete(dto);

            await DisplayAlert("Création Société", result, "OK");

            // Redirection après succès
            await Navigation.PushAsync(new Login());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        finally
        {
            // Réactivation systématique
            _isCreating = false;
            BTNVALIDER.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}
