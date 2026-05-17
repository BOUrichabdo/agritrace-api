using TracAgriApi.DTOs;
using TracAgriApi.Models;

namespace TracAgriApi.Services
{
    public interface IStockService
    {

        Task<SortieStock> CreateSortieAsync(CreateSortieStockDto dto);

        Task<PaletteSortieDto?> GetPaletteByCodeAsync(string code);
    }
}
