using AgriTraceApp.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class ReceptionService
    {
        private readonly HttpClient _httpClient;

        public ReceptionService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };
        }

        // =========================
        // CREER RECEPTION
        // =========================
        public async Task<ReceptionResponseDto?> CreateReception(
            CreateReceptionDto dto)
        {
            try
            {
                var response =
                    await _httpClient.PostAsJsonAsync(
                        "Receptions",
                        dto);

                if (!response.IsSuccessStatusCode)
                {
                    var error =
                        await response.Content.ReadAsStringAsync();

                    throw new Exception(error);
                }

                var result =
                    await response.Content
                        .ReadFromJsonAsync<ReceptionResponseDto>();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
        }




        public async Task<byte[]> PrintPalette(int id)
        {
            return await _httpClient.GetByteArrayAsync($"Printer/palette/{id}");
        }
        //public async Task<byte[]> GetReceptionPdf(int id)
        //{
        //    return await _httpClient.GetByteArrayAsync($"/api/Printer/printreception/{id}");
        //}


        // =========================
        // RÉCUPÉRER TOUTES LES RÉCEPTIONS
        // =========================
        public async Task<List<ReceptionResponseDto>> GetReceptions()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Receptions");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var receptions = JsonSerializer.Deserialize<List<ReceptionResponseDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return receptions ?? new List<ReceptionResponseDto>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erreur API: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetReceptions: {ex.Message}");
                throw;
            }
        }

        // =========================
        // RÉCUPÉRER LES RÉCEPTIONS AVEC FILTRES
        // =========================
        public async Task<List<ReceptionResponseDto>> GetReceptionsFiltered(DateTime? dateDebut, DateTime? dateFin, string? produit, string? agriculteur)
        {
            try
            {
                var query = new List<string>();
                if (dateDebut.HasValue)
                    query.Add($"dateDebut={dateDebut.Value:yyyy-MM-dd}");
                if (dateFin.HasValue)
                    query.Add($"dateFin={dateFin.Value:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(produit) && produit != "Tous")
                    query.Add($"produit={Uri.EscapeDataString(produit)}");
                if (!string.IsNullOrEmpty(agriculteur) && agriculteur != "Tous")
                    query.Add($"agriculteur={Uri.EscapeDataString(agriculteur)}");

                string url = "api/Receptions/filter";
                if (query.Any())
                    url += "?" + string.Join("&", query);

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ReceptionResponseDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<ReceptionResponseDto>();
                }

                return new List<ReceptionResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetReceptionsFiltered: {ex.Message}");
                return new List<ReceptionResponseDto>();
            }
        }

        // =========================
        // RÉCUPÉRER UNE RÉCEPTION PAR ID
        // =========================
        public async Task<ReceptionResponseDto?> GetReceptionById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Receptions/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var reception = JsonSerializer.Deserialize<ReceptionResponseDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return reception;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetReceptionById: {ex.Message}");
                return null;
            }
        }

    }
}
