

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
            // root 
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };
        }


        // =========================
        // GET ALL recuprere (toute les agris) par filtrage par ID societe
        // =========================


        public async Task<List<AgriculteurModel>> GetAgriculteurs(int societeId)
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<AgriculteurModel>>($"Agriculteur?societeId={societeId}");
            return result ?? new List<AgriculteurModel>();
        }

        // =========================
        // GET ALL recuprere (toute les agris)
        // =========================

        //public async Task<List<AgriculteurModel>> GetAgriculteurs()
        //{
        //    var result = await _httpClient
        //        .GetFromJsonAsync<List<AgriculteurModel>>("Agriculteur");

        //    return result ?? new List<AgriculteurModel>();
        //}


        // =========================
        // GET BY ID recup Agri avec ID et ID societe 
        // =========================

        public async Task<AgriculteurModel?> GetAgriculteurById(int id, int societeId)
        {
            return await _httpClient
                .GetFromJsonAsync<AgriculteurModel>($"Agriculteur/{id}?societeId={societeId}");
        }
        // =========================
        // GET BY ID recup Agri avec ID 
        // =========================

        public async Task<AgriculteurModel?> GetAgriculteurById(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<AgriculteurModel>($"Agriculteur/{id}");
        }

        // =========================
        // ADD ajouter agri avec ID societe 
        // =========================

        public async Task AddAgriculteur(string nom, string adresse, string telephone, int societeId)
        {
            var dto = new { Nom = nom, Adresse = adresse, Telephone = telephone };
            var response = await _httpClient
                .PostAsJsonAsync($"Agriculteur?societeId={societeId}", dto);
            response.EnsureSuccessStatusCode();
        }

        // =========================
        // ADD ajouter agri
        // =========================

        //public async Task AddAgriculteur(AgriculteurModel model)
        //{
        //    var response = await _httpClient
        //        .PostAsJsonAsync("Agriculteur", model);

        //    response.EnsureSuccessStatusCode();
        //}

        // =========================
        // UPDATE
        // =========================

        public async Task UpdateAgriculteur(int id, string nom, string adresse, string telephone, int societeId)
        {
            var dto = new { Id = id, Nom = nom, Adresse = adresse, Telephone = telephone };
            var response = await _httpClient
                .PutAsJsonAsync($"Agriculteur/{id}?societeId={societeId}", dto);
            response.EnsureSuccessStatusCode();
        }



        // =========================
        // DELETE  par ID et ID societe
        // =========================


        public async Task DeleteAgriculteur(int id,int societeId)
        {
            var response = await _httpClient
                .DeleteAsync($"Agriculteur/{id}?societeId={societeId}");
            response.EnsureSuccessStatusCode();
        }




        // =========================
        // DELETE
        // =========================

        //public async Task DeleteAgriculteur(int id)
        //{
        //    var response = await _httpClient
        //        .DeleteAsync($"Agriculteur/{id}");

        //    response.EnsureSuccessStatusCode();
        //}

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
