
using AgriTraceApp.Models;
using System.Net.Http.Json;

namespace AgriTraceApp.Services
{
    public class FermeService
    {
        // root
        private readonly HttpClient _httpClient;

        public FermeService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };
        }

        // =========================
        // GET ALL (par société)  Recuprere les fermes par ID societe
        // =========================
        public async Task<List<FermeModele>> GetFermes(int societeId)
        {
            var result = await _httpClient.GetFromJsonAsync<List<FermeModele>>($"Ferme?societeId={societeId}");
            return result ?? new List<FermeModele>();
        }

        // =========================
        // GET BY ID (par société) recuprere une ferme par ID et ID societe
        // =========================
        public async Task<FermeModele?> GetFermeById(int id, int societeId)
        {
            return await _httpClient.GetFromJsonAsync<FermeModele>($"Ferme/{id}?societeId={societeId}");

        }

        // =========================
        // ADD (avec societeId en query)
        // =========================
        //public async Task AddFerme(FermeModele model, int societeId)
        //{
        //    var response = await _httpClient
        //        .PostAsJsonAsync($"Ferme?societeId={societeId}", model);
        //    response.EnsureSuccessStatusCode();
        //}

        // ajouter Ferme avec gestion d'erreur détaillée
        public async Task AddFerme(FermeModele model, int societeId)
        {
            var response = await _httpClient.PostAsJsonAsync($"Ferme?societeId={societeId}", model);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erreur {response.StatusCode} : {errorContent}");
            }
        }

        // =========================
        // UPDATE (avec societeId) modification d'une ferme avec gestion d'erreur détaillée
        // =========================
        public async Task UpdateFerme(FermeModele model, int societeId)
        {
            var response = await _httpClient
                .PutAsJsonAsync($"Ferme/{model.Id}?societeId={societeId}", model);
            response.EnsureSuccessStatusCode();
        }

        // =========================
        // DELETE (avec societeId)  Suppression d'une ferme
        // =========================
        public async Task DeleteFerme(int id, int societeId)
        {
            var response = await _httpClient.DeleteAsync($"Ferme/{id}?societeId={societeId}");

            response.EnsureSuccessStatusCode();
        }
    }
}













//using AgriTraceApp.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Json;
//using System.Text;
//using System.Threading.Tasks;

//namespace AgriTraceApp.Services
//{
//    public class FermeService
//    {
//        private readonly HttpClient _httpClient;

//        public FermeService()
//        {
//            _httpClient = new HttpClient
//            {
//                BaseAddress = new Uri(ApiConfig.BaseUrl)
//            };
//        }

//        // =========================
//        // GET ALL
//        // =========================

//        public async Task<List<FermeModele>> GetFermes()
//        {
//            var result = await _httpClient
//                .GetFromJsonAsync<List<FermeModele>>("Ferme");

//            return result ?? new List<FermeModele>();
//        }

//        // =========================
//        // GET BY ID
//        // =========================

//        public async Task<FermeModele?> GetFermeById(int id)
//        {
//            return await _httpClient
//                .GetFromJsonAsync<FermeModele>($"Ferme/{id}");
//        }

//        // =========================
//        // ADD
//        // =========================

//        public async Task AddFerme(FermeModele model)
//        {
//            var response = await _httpClient
//                .PostAsJsonAsync("Ferme", model);

//            response.EnsureSuccessStatusCode();
//        }

//        // =========================
//        // UPDATE
//        // =========================

//        public async Task UpdateFerme(FermeModele model)
//        {
//            var response = await _httpClient
//                .PutAsJsonAsync($"Ferme/{model.Id}", model);

//            response.EnsureSuccessStatusCode();
//        }

//        // =========================
//        // DELETE
//        // =========================

//        public async Task DeleteFerme(int id)
//        {
//            var response = await _httpClient
//                .DeleteAsync($"Ferme/{id}");

//            response.EnsureSuccessStatusCode();
//        }
//    }
//}
