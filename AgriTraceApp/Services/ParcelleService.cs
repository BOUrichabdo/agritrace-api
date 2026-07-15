using AgriTraceApp.Models;
using System.Net.Http.Json;

namespace AgriTraceApp.Services
{
    public class ParcelleService
    {
        private readonly HttpClient _httpClient;

        public ParcelleService()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri(ApiConfig.BaseUrl);

        }

        // =========================
        // GET ALL PARCELLES par Id socuiete 
        // =========================


        public async Task<List<ParcelleModel>> GetParcelle(int societeId)
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<ParcelleModel>>($"Parcelle?societeId={societeId}");
            return result ?? new List<ParcelleModel>();
        }
        //public async Task<List<ParcelleModel>> GetParcelle(int societeId)
        //{
        //    var result =
        //        await _httpClient.GetFromJsonAsync<List<ParcelleModel>>
        //        ("Parcelle");

        //    return result ?? new List<ParcelleModel>();
        //}




        // =========================
        // AJOUTER PARCELLE
        // =========================

        public async Task Addparcelle(ParcelleModel model, int societeId)
        {
            var response = await _httpClient
               .PostAsJsonAsync($"Parcelle?societeId={societeId}", model);

            response.EnsureSuccessStatusCode();
        }



        // comment realiser cette partie pour ajouter un agriculteur avec societeId
        //public async Task Addparcelle(string nom, string adresse, string telephone, int societeId)
        //{
        //    var dto = new { Nom = nom, Adresse = adresse, Telephone = telephone };
        //    var response = await _httpClient
        //        .PostAsJsonAsync($"Agriculteur?societeId={societeId}", dto);
        //    response.EnsureSuccessStatusCode();
        //}








        // =========================
        // MODIFIER PARCELLE
        // =========================

        public async Task UpdateParcelled(ParcelleModel model)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"Parcelle/{model.Id}",
                    model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // DELETE PARCELLE
        // =========================

        public async Task DeletParacelle(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"Parcelle/{id}");

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // GET PARCELLE BY FERME
        // =========================

        public async Task<List<ParcelleModel>>
            GetParcelleByFerme(int fermeId)
        {
            var result =
                await _httpClient.GetFromJsonAsync<List<ParcelleModel>>
                ($"Parcelle/byparcelle/{fermeId}");

            return result ?? new List<ParcelleModel>();
        }
    }
}
