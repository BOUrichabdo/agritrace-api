using AgriTraceApp.Models;
using System.Net.Http.Json;

namespace AgriTraceApp.Services
{
    public class VarieteService
    {
        private readonly HttpClient _httpClient;

        public VarieteService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiConfig.BaseUrl);
        }

        // GET ALL avec societeId
        public async Task<List<ModeleVariete>> GetVarietes(int societeId)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ModeleVariete>>($"Varietes?societeId={societeId}");
            return result ?? new List<ModeleVariete>();
        }

        // GET BY ID avec societeId
        public async Task<ModeleVariete?> GetVarieteById(int id, int societeId)
        {
            return await _httpClient.GetFromJsonAsync<ModeleVariete>($"Varietes/{id}?societeId={societeId}");
        }

        // AJOUTER avec societeId
        public async Task AddVariete(ModeleVariete model, int societeId)
        {
            var response = await _httpClient.PostAsJsonAsync($"Varietes?societeId={societeId}", model);
            response.EnsureSuccessStatusCode();
        }



        // MODIFIER avec societeId
        public async Task UpdateVariete(ModeleVariete model,int societeId)
        {
            var response = await _httpClient.PutAsJsonAsync($"Varietes/{model.Id}?societeId={societeId}", model);
            response.EnsureSuccessStatusCode();
        }

        // DELETE avec societeId
        public async Task DeleteVariete(int id, int societeId)
        {
            var response = await _httpClient.DeleteAsync($"Varietes/{id}?societeId={societeId}");
            response.EnsureSuccessStatusCode();
        }
    }
}




































//using AgriTraceApp.Models;
//using System.Net.Http.Json;

//namespace AgriTraceApp.Services
//{
//    public class VarieteService
//    {
//        private readonly HttpClient _httpClient;

//        public VarieteService()
//        {
//            _httpClient = new HttpClient();

//            _httpClient.BaseAddress =
//                new Uri(ApiConfig.BaseUrl);
//        }

//        // =========================
//        // GET ALL
//        // =========================

//        public async Task<List<ModeleVariete>> GetVarietes()
//        {
//            var result =
//                await _httpClient.GetFromJsonAsync<List<ModeleVariete>>
//                ("Varietes");

//            return result ?? new List<ModeleVariete>();
//        }

//        // =========================
//        // GET BY ID
//        // =========================

//        public async Task<ModeleVariete?> GetVarieteById(int id)
//        {
//            return await _httpClient.GetFromJsonAsync<ModeleVariete>
//                ($"Varietes/{id}");
//        }

//        // =========================
//        // AJOUTER
//        // =========================

//        public async Task AddVariete(ModeleVariete model)
//        {
//            var response =
//                await _httpClient.PostAsJsonAsync(
//                    "Varietes",
//                    model);

//            response.EnsureSuccessStatusCode();
//        }

//        // =========================
//        // MODIFIER
//        // =========================

//        public async Task UpdateVariete(ModeleVariete model)
//        {
//            var response =
//                await _httpClient.PutAsJsonAsync(
//                    $"Varietes/{model.Id}",
//                    model);

//            response.EnsureSuccessStatusCode();
//        }

//        // =========================
//        // DELETE
//        // =========================

//        public async Task DeleteVariete(int id)
//        {
//            var response =
//                await _httpClient.DeleteAsync(
//                    $"Varietes/{id}");

//            response.EnsureSuccessStatusCode();
//        }
//    }
//}
