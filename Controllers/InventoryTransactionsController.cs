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
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IInventoryTransactionService _service;

        public InventoryTransactionsController(IInventoryTransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<InventoryTransactionDto>>> GetTransactions([FromQuery] PagedQuery query)
        {
            return await _service.GetAsync(query);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<InventoryTransactionDto>> GetTransaction(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return dto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryTransactionDto>> PostTransaction(InventoryTransactionCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.error != null) return BadRequest(result.error);
            return CreatedAtAction(nameof(GetTransaction), new { id = result.dto!.Id }, result.dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTransaction(int id, InventoryTransactionUpdateDto dto)
        {
            var error = await _service.UpdateAsync(id, dto);
            if (error == "Not found.") return NotFound();
            if (error != null) return BadRequest(error);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var error = await _service.DeleteAsync(id);
            if (error == "Not found.") return NotFound();
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}
