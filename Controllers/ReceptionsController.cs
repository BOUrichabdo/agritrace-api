using Microsoft.AspNetCore.Mvc;
using TracAgriApi.DTOs;
using TracAgriApi.Servises;

namespace TracAgriApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionsController : ControllerBase
    {
        // contrat interface Reception service pour deleguer la logique metier a une classe de service
        private readonly IReceptionService _service;
        public ReceptionsController(IReceptionService service)
        {
            _service = service;
        }


        // creation d'une reception a partir d'un DTO de creation
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReceptionDto dto)
        {
            try
            {
                var result = await _service.CreateReceptionAsync(dto);
                return Ok(result);
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
