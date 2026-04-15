using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Ecom.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> Get()
        {
            var categories = await unitOfWork.CategoryRepository.GetAllAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound(new { Message = "No categories found." });
            }
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(new { Message = "Category data is null." });
            }

            var category = mapper.Map<Category>(categoryDto);
            await unitOfWork.CategoryRepository.AddAsync(category);
            return Ok(new { Message = "Category added successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(new { Message = "Category data is null." });
            }

            var existingCategory = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }

            mapper.Map(categoryDto, existingCategory);
            await unitOfWork.CategoryRepository.UpdateAsync(existingCategory);

            return Ok(new { Message = "Category updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var existingCategory = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }

            await unitOfWork.CategoryRepository.DeleteAsync(id);
            return Ok(new { Message = "Category deleted successfully." });
        }
    }
}
