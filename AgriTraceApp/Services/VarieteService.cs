using AgriTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class VarieteService
    {
        private readonly HttpClient _httpClient;

        public VarieteService()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress =
                new Uri(ApiConfig.BaseUrl);
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<ModeleVariete>> GetVarietes()
        {
            var result =
                await _httpClient.GetFromJsonAsync<List<ModeleVariete>>
                ("Varietes");

            return result ?? new List<ModeleVariete>();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<ModeleVariete?> GetVarieteById(int id)
        {
            return await _httpClient.GetFromJsonAsync<ModeleVariete>
                ($"Varietes/{id}");
        }

        // =========================
        // AJOUTER
        // =========================

        public async Task AddVariete(ModeleVariete model)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "Varietes",
                    model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // MODIFIER
        // =========================

        public async Task UpdateVariete(ModeleVariete model)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"Varietes/{model.Id}",
                    model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // DELETE
        // =========================

        public async Task DeleteVariete(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"Varietes/{id}");

            response.EnsureSuccessStatusCode();
        }
    }
}
