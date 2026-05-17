using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TracAgriApi.DTOs;
using TracAgriApi.Services;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortieStockController : ControllerBase
    {

        private readonly IStockService _service;

        public SortieStockController(IStockService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSortieStockDto dto)
        {
            try
            {
                var result = await _service.CreateSortieAsync(dto);

                return Ok(new
                {
                    message = "Sortie stock réussie",
                    id = result.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
