using AgriTraceApp.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class EtiquetteFermeService
    {
        private readonly HttpClient _httpClient;

        public EtiquetteFermeService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };
        }

        // =========================
        // GENERER ETIQUETTE
        // =========================
        public async Task<EtiquetteDto?> GenererEtiquette(
            CreateEtiquetteDto dto)
        {
            try
            {
                var response =
                    await _httpClient.PostAsJsonAsync(
                        "EtiquetteFerme/generer",
                        dto);

                if (!response.IsSuccessStatusCode)
                    return null;

                var result =
                    await response.Content
                        .ReadFromJsonAsync<EtiquetteDto>();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        // =========================
        // GET ETIQUETTE BY CODE
        // =========================
        public async Task<EtiquetteDto?> GetEtiquetteByCode(
            string code)
        {
            try
            {
                return await _httpClient
                    .GetFromJsonAsync<EtiquetteDto>(
                        $"EtiquetteFerme/bycode/{code}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }


        public async Task<byte[]> GetPdf(int id)
        {

            try
            {
                // Utiliser l'URL Railway
                string url = $"https://agritrace-api-production.up.railway.app/api/Printer/print/{id}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API error {response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetPdf: {ex.Message}");
                throw;
            }



            return await _httpClient.GetByteArrayAsync($"api/Printer/print/{id}"); 
        }
    }
}

