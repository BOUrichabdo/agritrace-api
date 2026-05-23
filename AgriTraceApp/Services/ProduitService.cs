using AgriTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgriTraceApp.Services
{
    public class ProduitService
    {
        private readonly HttpClient _httpClient;

        public ProduitService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            };
        }

        // ============================
        // GET ALL PRODUITS
        // ============================
        public async Task<List<ProduitModel>> Getproduit()
        {
            try
            {
                var result = await _httpClient
                    .GetFromJsonAsync<List<ProduitModel>>("Produits");

                return result ?? new List<ProduitModel>();
            }
            catch
            {
                return new List<ProduitModel>();
            }
        }

        // ============================
        // GET PRODUIT BY ID
        // ============================
        public async Task<ProduitModel> GetProduitById(int id)
        {
            try
            {
                return await _httpClient
                    .GetFromJsonAsync<ProduitModel>(
                        $"Produits/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ============================
        // ADD PRODUIT
        // ============================
        public async Task Addproduit(ProduitModel produit)
        {
            await _httpClient.PostAsJsonAsync(
                "Produits",
                produit);
        }

        // ============================
        // UPDATE PRODUIT
        // ============================
        public async Task UpdateProduit(ProduitModel produit)
        {
            await _httpClient.PutAsJsonAsync(
                $"Produits/{produit.Id}",
                produit);
        }

        // ============================
        // DELETE PRODUIT
        // ============================
        public async Task Deleteproduit(int id)
        {
            await _httpClient.DeleteAsync(
                $"Produits/{id}");
        }

        // ============================
        // GET PRODUIT BY PARCELLE
        // ============================
        public async Task<List<ProduitModel>> GetProduitByParcelle(int parcelleId)
        {
            try
            {
                var result = await _httpClient
                    .GetFromJsonAsync<List<ProduitModel>>(
                        $"Produits/byparcelle/{parcelleId}");

                return result ?? new List<ProduitModel>();
            }
            catch
            {
                return new List<ProduitModel>();
            }
        }

        // ============================
        // GET VARIETE BY PRODUIT
        // ============================
        public async Task<ModeleVariete> GetVarieteByProduit(int varieteId)
        {
            try
            {
                return await _httpClient
                    .GetFromJsonAsync<ModeleVariete>(
                        $"Produits/byvariete/{varieteId}");
            }
            catch
            {
                return null;
            }
        }



        public async Task<List<ParcelleModel>> GetParcelle()
        {
            try
            {
                var data = await _httpClient
                    .GetFromJsonAsync<List<ParcelleModel>>("Parcelle");

                return data ?? new List<ParcelleModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ParcelleModel>();
            }
        }

        // =========================
        // GET VARIETES
        // =========================
        public async Task<List<ModeleVariete>> GetVarietes()
        {
            try
            {
                var data = await _httpClient
                    .GetFromJsonAsync<List<ModeleVariete>>("Varietes");

                return data ?? new List<ModeleVariete>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ModeleVariete>();
            }
        }
    }
}

