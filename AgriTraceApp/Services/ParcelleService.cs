using AgriTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class ParcelleService
    {
        private readonly HttpClient _httpClient;

        public ParcelleService()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress =
                new Uri(ApiConfig.BaseUrl);
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

        public async Task Addparcelle(ParcelleModel model)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "Parcelle",
                    model);

            response.EnsureSuccessStatusCode();
        }

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
