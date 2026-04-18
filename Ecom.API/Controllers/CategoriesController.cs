using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Ecom.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        // ========================= GET ALL =========================
        [HttpGet]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> Get()
        {
            var categories = await unitOfWork.CategoryRepository.GetAllAsync();

            //var result = mapper.Map<IEnumerable<CategoryDTO>>(categories);

            return Ok(categories);
        }

        // ========================= GET BY ID =========================
        [HttpGet("{id}")]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound(new { Message = $"Category with ID {id} not found." });

            var result = mapper.Map<CategoryDTO>(category);

            return Ok(result);
        }

        // ========================= CREATE =========================
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
                return BadRequest(new { Message = "Category data is null." });

            var category = mapper.Map<Category>(categoryDto);

            await unitOfWork.CategoryRepository.AddAsync(category);

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                mapper.Map<CategoryDTO>(category)
            );
        }

        // ========================= UPDATE =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
                return BadRequest(new { Message = "Category data is null." });

            var existingCategory = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (existingCategory == null)
                return NotFound(new { Message = $"Category with ID {id} not found." });

            mapper.Map(categoryDto, existingCategory);

            await unitOfWork.CategoryRepository.UpdateAsync(existingCategory);

            return NoContent();
        }

        // ========================= DELETE =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var existingCategory = await unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (existingCategory == null)
                return NotFound(new { Message = $"Category with ID {id} not found." });

            await unitOfWork.CategoryRepository.DeleteAsync(existingCategory.Id);

            return NoContent();
        }
    }
}