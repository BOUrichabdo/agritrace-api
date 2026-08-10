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
               

                var inner = ex.InnerException;
                while (inner?.InnerException != null)
                    inner = inner.InnerException;

                var errorMessage = inner?.Message ?? ex.Message;

                return BadRequest(new
                {
                    message = errorMessage,
                    full = ex.ToString()   // optionnel, pour avoir tout le stack trace
                });

            
            }
        }
    }
}
