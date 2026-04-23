using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DealersController : ControllerBase
    {
        private readonly IDealerService _service;

        public DealersController(IDealerService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<DealerDto>>> GetDealers([FromQuery] PagedQuery query)
        {
            return await _service.GetAsync(query);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DealerDto>> GetDealer(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return dto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DealerDto>> PostDealer(DealerCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.error != null) return BadRequest(result.error);
            return CreatedAtAction(nameof(GetDealer), new { id = result.dto!.Id }, result.dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutDealer(int id, DealerUpdateDto dto)
        {
            var error = await _service.UpdateAsync(id, dto);
            if (error == "Not found.") return NotFound();
            if (error != null) return BadRequest(error);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDealer(int id)
        {
            var error = await _service.DeleteAsync(id);
            if (error == "Not found.") return NotFound();
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}
