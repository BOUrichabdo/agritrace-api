using AgriTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class AgriculteurService
    {
        private readonly HttpClient _httpClient;

        public AgriculteurService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<AgriculteurModel>> GetAgriculteurs()
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<AgriculteurModel>>("Agriculteur");

            return result ?? new List<AgriculteurModel>();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<AgriculteurModel?> GetAgriculteurById(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<AgriculteurModel>($"Agriculteur/{id}");
        }

        // =========================
        // ADD
        // =========================

        public async Task AddAgriculteur(AgriculteurModel model)
        {
            var response = await _httpClient
                .PostAsJsonAsync("Agriculteur", model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // UPDATE
        // =========================

        public async Task UpdateAgriculteur(AgriculteurModel model)
        {
            var response = await _httpClient
                .PutAsJsonAsync($"Agriculteur/{model.Id}", model);

            response.EnsureSuccessStatusCode();
        }

        // =========================
        // DELETE
        // =========================

        public async Task DeleteAgriculteur(int id)
        {
            var response = await _httpClient
                .DeleteAsync($"Agriculteur/{id}");

            response.EnsureSuccessStatusCode();
        }

        // GET FERMES BY AGRICULTEUR
        // =========================
        public async Task<List<FermeModele>> GetFermesByAgriculteur(int agriculteurId)
        {
            try
            {
                var data = await _httpClient
                    .GetFromJsonAsync<List<FermeModele>>(
                        $"Agriculteur/byAgriculteur/{agriculteurId}");

                return data ?? new List<FermeModele>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<FermeModele>();
            }
        }
    }
}
