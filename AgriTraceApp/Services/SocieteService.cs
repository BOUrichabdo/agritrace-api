using AgriTraceApp.DTOs;
using System.Net.Http.Json;

namespace AgriTraceApp.Services;

public class SocieteService
{
    private readonly HttpClient _httpClient;

    public SocieteService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiConfig.BaseUrl)
        };
    }



    //public async Task<string> CreateSociete(CreateSocieteDto dto)
    //{
    //    var response =
    //        await _httpClient.PostAsJsonAsync(
    //            "api/societe",
    //            dto);

    //    var content =
    //        await response.Content.ReadAsStringAsync();

    //    return content;
    //}


    public async Task<string> CreateSociete(CreateSocieteDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/societe", dto); 

        var content = await response.Content.ReadAsStringAsync();

        return $"Status: {response.StatusCode}\n\n{content}";
    }
}