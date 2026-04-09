using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Services;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _service;

        public StocksController(IStockService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<StockDto>>> GetStocks([FromQuery] PagedQuery query)
        {
            return await _service.GetAsync(query);
        }

        [HttpGet("{warehouseId:int}/{productId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<StockDto>> GetStock(int warehouseId, int productId)
        {
            var dto = await _service.GetAsync(warehouseId, productId);
            if (dto == null) return NotFound();
            return dto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StockDto>> UpsertStock(StockUpsertDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            if (result.error != null) return BadRequest(result.error);
            return Ok(result.dto);
        }

        [HttpDelete("{warehouseId:int}/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStock(int warehouseId, int productId)
        {
            var error = await _service.DeleteAsync(warehouseId, productId);
            if (error == "Not found.") return NotFound();
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}
