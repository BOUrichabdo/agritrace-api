using AgriTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class CategorieService
    {
        private readonly HttpClient _httpClient;

        public CategorieService()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress =
                new Uri(ApiConfig.BaseUrl);
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<ModeleCategorie>> GetCategorie()
        {
            var result =
                await _httpClient.GetFromJsonAsync<List<ModeleCategorie>>
                ("Categories");

            return result ?? new List<ModeleCategorie>();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<ModeleCategorie?> GetCategorieById(int id)
        {
            return await _httpClient.GetFromJsonAsync<ModeleCategorie>
                ($"Categories/{id}");
        }

        // =========================
        // AJOUTER
        // =========================

        public async Task AddCategorie(ModeleCategorie model)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "Categories",
                    model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // MODIFIER
        // =========================

        public async Task UpdateCategorie(ModeleCategorie model)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"Categories/{model.Id}",
                    model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // DELETE
        // =========================

        public async Task DeletCategorie(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"Categories/{id}");

            response.EnsureSuccessStatusCode();
        }
    }
}
