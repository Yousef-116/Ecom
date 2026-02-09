using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> get()
        {
            try
            {
                var categories = await unitOfWork.CategoryRepository.GetAllAsync();
                if (categories == null || categories.Count() == 0)
                {
                    return NotFound("No categories found.");
                }
                else
                {
                    return Ok(categories);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving categories: {ex.Message}");
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                else
                {
                    return Ok(category);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the category: {ex.Message}");
            }
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> addCategory([FromBody] CategoryDTO category)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest("Category data is null.");
                }

                Category newCategory = mapper.Map<Category>(category);
                await unitOfWork.CategoryRepository.AddAsync(newCategory);
                return Ok("Category added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the category: {ex.Message}");
            }
        }


        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> updateCategory(int id, [FromBody] CategoryDTO category)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest("Category data is null.");
                }

                var existingCategory = await unitOfWork.CategoryRepository.GetByIdAsync(id);
                Console.WriteLine(existingCategory);
                if (existingCategory == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                // 🔥 THIS is the key
                mapper.Map(category, existingCategory);

                await unitOfWork.CategoryRepository.UpdateAsync(existingCategory);
                return Ok("Category updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the category: {ex.Message}");
            }


        }
        // [HttpPut("update-category")]
        // public async Task<IActionResult> updateCategory([FromBody] UpdateCategoryDTO category)
        // {
        //     try
        //     {
        //         if (category == null)
        //         {
        //             return BadRequest("Category data is null.");
        //         }

        //         var existingCategory = await unitOfWork.CategoryRepository.GetByIdAsync(category.id);
        //         if (existingCategory == null)
        //         {
        //             return NotFound($"Category with ID {category.id} not found.");
        //         }

        //         existingCategory.Name = category.Name;
        //         existingCategory.Description = category.Description;

        //         await unitOfWork.CategoryRepository.UpdateAsync(existingCategory);
        //         return Ok("Category updated successfully.");
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"An error occurred while updating the category: {ex.Message}");
        //     }


        // }

        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> deleteCategory(int id)
        {
            try
            {
                var existingCategory = await unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                await unitOfWork.CategoryRepository.DeleteAsync(id);
                return Ok("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the category: {ex.Message}");
            }
        }




    }
}
