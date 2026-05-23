using AgriTraceApp.DTOs;
using System.Net.Http.Json;

namespace AgriTraceApp.Services
{
    public class SortieStockService
    {
        private readonly HttpClient _httpClient;

        public SortieStockService()
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress =
                new Uri(ApiConfig.BaseUrl);
        }

        // =========================
        // RECHERCHE PALETTE
        // =========================

        public async Task<PaletteSortieDto?> GetPaletteByCode(string code)
        {
            try
            {
                return await _httpClient
                    .GetFromJsonAsync<PaletteSortieDto>(
                    $"Stocks/palette/{code}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        // =========================
        // VALIDATION SORTIE
        // =========================

        public async Task<bool> CreateSortie(
            CreateSortieStockDto dto)
        {
            try
            {
                var response = await _httpClient
                    .PostAsJsonAsync(
                    "SortieStock",
                    dto);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }
    }
}