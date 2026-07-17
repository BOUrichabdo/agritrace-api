using AgriTraceApp.Models;
using Android.Util;
using System.Net.Http.Json;

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
        public async Task<List<ProduitModel>> GetProduit(int societeId)
        {
            try
            {
                var result = await _httpClient
                    .GetFromJsonAsync<List<ProduitModel>>($"Produits?societeId={societeId}");
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
        public async Task<ProduitModel?> GetProduitById(int id, int societeId)
        {
            try
            {
                return await _httpClient
                    .GetFromJsonAsync<ProduitModel>($"Produits/{id}?societeId={societeId}");
            }
            catch
            {
                return null;
            }
        }

        // ============================
        // ADD PRODUIT
        // ============================
        public async Task AddProduit(ProduitModel produit, int societeId)
        {
            await _httpClient.PostAsJsonAsync($"Produits?societeId={societeId}", produit);
        }

        // ============================
        // UPDATE PRODUIT
        // ============================
        public async Task UpdateProduit(ProduitModel produit, int societeId)
        {
            await _httpClient.PutAsJsonAsync($"Produits/{produit.Id}?societeId={societeId}", produit);
        }

        // ============================
        // DELETE PRODUIT
        // ============================
        public async Task DeleteProduit(int id, int societeId)
        {
            await _httpClient.DeleteAsync($"Produits/{id}?societeId={societeId}");
        }

        // ============================
        // GET PRODUIT BY PARCELLE
        // ============================
        public async Task<List<ProduitModel>> GetProduitByParcelle(int parcelleId, int societeId)
        {
            try
            {
                var result = await _httpClient
                    .GetFromJsonAsync<List<ProduitModel>>($"Produits/byparcelle/{parcelleId}?societeId={societeId}");
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
        public async Task<ModeleVariete?> GetVarieteByProduit(int varieteId, int societeId)
        {
            try
            {
                return await _httpClient
                    .GetFromJsonAsync<ModeleVariete>($"Produits/byvariete/{varieteId}?societeId={societeId}");
            }
            catch
            {
                return null;
            }
        }

        // ============================
        // GET PARCELLES (avec filtrage société)
        // ============================
        public async Task<List<ParcelleModel>> GetParcelle(int societeId)
        {
            try
            {
                var data = await _httpClient
                    .GetFromJsonAsync<List<ParcelleModel>>($"Parcelle?societeId={societeId}");
                return data ?? new List<ParcelleModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ParcelleModel>();
            }
        }

        // ============================
        // GET VARIETES (avec filtrage société)
        // ============================
        public async Task<List<ModeleVariete>> GetVarietes(int societeId)
        {
            try
            {
                var data = await _httpClient
                    .GetFromJsonAsync<List<ModeleVariete>>($"Varietes?societeId={societeId}");
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




































//using AgriTraceApp.Models;
//using System.Net.Http.Json;

//namespace AgriTraceApp.Services
//{
//    public class ProduitService
//    {
//        // root
//        private readonly HttpClient _httpClient;

//        public ProduitService()
//        {
//            _httpClient = new HttpClient
//            {
//                BaseAddress = new Uri(ApiConfig.BaseUrl)
//            };
//        }

//        // ============================
//        // GET ALL PRODUITS
//        // ============================
//        public async Task<List<ProduitModel>> Getproduit()
//        {
//            try
//            {
//                var result = await _httpClient
//                    .GetFromJsonAsync<List<ProduitModel>>("Produits");

//                return result ?? new List<ProduitModel>();
//            }
//            catch
//            {
//                return new List<ProduitModel>();
//            }
//        }

//        // ============================
//        // GET PRODUIT BY ID
//        // ============================
//        public async Task<ProduitModel> GetProduitById(int id)
//        {
//            try
//            {
//                return await _httpClient
//                    .GetFromJsonAsync<ProduitModel>(
//                        $"Produits/{id}");
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        // ============================
//        // ADD PRODUIT
//        // ============================
//        public async Task Addproduit(ProduitModel produit)
//        {
//            await _httpClient.PostAsJsonAsync(
//                "Produits",
//                produit);
//        }

//        // ============================
//        // UPDATE PRODUIT
//        // ============================
//        public async Task UpdateProduit(ProduitModel produit)
//        {
//            await _httpClient.PutAsJsonAsync(
//                $"Produits/{produit.Id}",
//                produit);
//        }

//        // ============================
//        // DELETE PRODUIT
//        // ============================
//        public async Task Deleteproduit(int id)
//        {
//            await _httpClient.DeleteAsync(
//                $"Produits/{id}");
//        }

//        // ============================
//        // GET PRODUIT BY PARCELLE
//        // ============================
//        public async Task<List<ProduitModel>> GetProduitByParcelle(int parcelleId)
//        {
//            try
//            {
//                var result = await _httpClient
//                    .GetFromJsonAsync<List<ProduitModel>>(
//                        $"Produits/byparcelle/{parcelleId}");

//                return result ?? new List<ProduitModel>();
//            }
//            catch
//            {
//                return new List<ProduitModel>(); ;
//            }
//        }

//        // ============================
//        // GET VARIETE BY PRODUIT
//        // ============================
//        public async Task<ModeleVariete> GetVarieteByProduit(int varieteId)
//        {
//            try
//            {
//                return await _httpClient
//                    .GetFromJsonAsync<ModeleVariete>(
//                        $"Produits/byvariete/{varieteId}");
//            }
//            catch
//            {
//                return null;
//            }
//        }


//        public async Task<List<ParcelleModel>> GetParcelle()
//        {
//            try
//            {
//                var data = await _httpClient
//                    .GetFromJsonAsync<List<ParcelleModel>>("Parcelle");

//                return data ?? new List<ParcelleModel>();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                return new List<ParcelleModel>();
//            }
//        }

//        // =========================
//        // GET VARIETES
//        // =========================
//        public async Task<List<ModeleVariete>> GetVarietes()
//        {
//            try
//            {
//                var data = await _httpClient
//                    .GetFromJsonAsync<List<ModeleVariete>>("Varietes");

//                return data ?? new List<ModeleVariete>();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                return new List<ModeleVariete>();
//            }
//        }
//    }
//}

