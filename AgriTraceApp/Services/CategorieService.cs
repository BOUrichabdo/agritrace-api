
using AgriTraceApp.Models;
using System.Net.Http.Json;

namespace AgriTraceApp.Services
{
    public class CategorieService
    {
        private readonly HttpClient _httpClient;

        public CategorieService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiConfig.BaseUrl);
        }

        // GET ALL avec societeId
        public async Task<List<ModeleCategorie>> GetCategorie(int societeId)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ModeleCategorie>>($"Categories?societeId={societeId}");
            return result ?? new List<ModeleCategorie>();
        }

        // GET BY ID avec societeId
        public async Task<ModeleCategorie?> GetCategorieById(int id, int societeId)
        {
            return await _httpClient.GetFromJsonAsync<ModeleCategorie>($"Categories/{id}?societeId={societeId}");
        }

        // AJOUTER avec societeId
        public async Task AddCategorie(ModeleCategorie model, int societeId)
        {
            var response = await _httpClient.PostAsJsonAsync($"Categories?societeId={societeId}", model);
            response.EnsureSuccessStatusCode();
        }

        // MODIFIER avec societeId
        public async Task UpdateCategorie(ModeleCategorie model, int societeId)
        {
            var response = await _httpClient.PutAsJsonAsync($"Categories/{model.Id}?societeId={societeId}", model);
            response.EnsureSuccessStatusCode();
        }

        // DELETE avec societeId
        public async Task DeletCategorie(int id, int societeId)
        {
            var response = await _httpClient.DeleteAsync($"Categories/{id}?societeId={societeId}");
            response.EnsureSuccessStatusCode();
        }
    }
}













//using AgriTraceApp.Models;
//using System.Net.Http.Json;

//namespace AgriTraceApp.Services
//{
//    public class CategorieService
//    {
//        private readonly HttpClient _httpClient;

//        public CategorieService()
//        {
//            _httpClient = new HttpClient();

//            _httpClient.BaseAddress =
//                new Uri(ApiConfig.BaseUrl);
//        }

//        // =========================
//        // GET ALL
//        // =========================

//        public async Task<List<ModeleCategorie>> GetCategorie()
//        {
//            var result =
//                await _httpClient.GetFromJsonAsync<List<ModeleCategorie>>
//                ("Categories");

//            return result ?? new List<ModeleCategorie>();
//        }

//        // =========================
//        // GET BY ID
//        // =========================

//        public async Task<ModeleCategorie?> GetCategorieById(int id)
//        {
//            return await _httpClient.GetFromJsonAsync<ModeleCategorie>
//                ($"Categories/{id}");
//        }

//        // =========================
//        // AJOUTER
//        // =========================

//        public async Task AddCategorie(ModeleCategorie model)
//        {
//            var response =
//                await _httpClient.PostAsJsonAsync(
//                    "Categories",
//                    model);

//            response.EnsureSuccessStatusCode();
//        }

//        // =========================
//        // MODIFIER
//        // =========================

//        public async Task UpdateCategorie(ModeleCategorie model)
//        {
//            var response =
//                await _httpClient.PutAsJsonAsync(
//                    $"Categories/{model.Id}",
//                    model);

//            response.EnsureSuccessStatusCode();
//        }

//        // =========================
//        // DELETE
//        // =========================

//        public async Task DeletCategorie(int id)
//        {
//            var response =
//                await _httpClient.DeleteAsync(
//                    $"Categories/{id}");

//            response.EnsureSuccessStatusCode();
//        }
//    }
//}
