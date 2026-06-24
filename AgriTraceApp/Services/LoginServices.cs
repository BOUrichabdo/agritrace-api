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

    public class LoginServices
    {
        // routing
        private readonly HttpClient _httpClient;
        public LoginServices()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };



        }

        public async Task<LoginResponseDto?> Login(LoginDto dto)
        {
            var json = JsonSerializer.Serialize(dto);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("auth/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            return result;
        }
    }
}
