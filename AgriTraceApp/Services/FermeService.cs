using AgriTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class FermeService
    {
        private readonly HttpClient _httpClient;

        public FermeService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<FermeModele>> GetFermes()
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<FermeModele>>("Ferme");

            return result ?? new List<FermeModele>();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<FermeModele?> GetFermeById(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<FermeModele>($"Ferme/{id}");
        }

        // =========================
        // ADD
        // =========================

        public async Task AddFerme(FermeModele model)
        {
            var response = await _httpClient
                .PostAsJsonAsync("Ferme", model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // UPDATE
        // =========================

        public async Task UpdateFerme(FermeModele model)
        {
            var response = await _httpClient
                .PutAsJsonAsync($"Ferme/{model.Id}", model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // DELETE
        // =========================

        public async Task DeleteFerme(int id)
        {
            var response = await _httpClient
                .DeleteAsync($"Ferme/{id}");

            response.EnsureSuccessStatusCode();
        }
    }
}
