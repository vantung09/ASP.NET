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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<CategoryDto>>> GetCategories([FromQuery] PagedQuery query)
        {
            return await _service.GetAsync(query);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return dto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.error != null) return BadRequest(result.error);
            return CreatedAtAction(nameof(GetCategory), new { id = result.dto!.Id }, result.dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCategory(int id, CategoryUpdateDto dto)
        {
            var error = await _service.UpdateAsync(id, dto);
            if (error == "Not found.") return NotFound();
            if (error != null) return BadRequest(error);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var error = await _service.DeleteAsync(id);
            if (error == "Not found.") return NotFound();
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}
