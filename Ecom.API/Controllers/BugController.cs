using System.ComponentModel;
using AutoMapper;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class BugController : BaseController
    {
        public BugController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        // [HttpGet("not-found")]
        // public async Task<IActionResult> GetNotFound()
        // {
        //     var category = await unitOfWork.CategoryRepository.GetByIdAsync(100);
        //     if(category == null) return NotFound();
        //     return Ok(category);
        // }
        // [HttpGet("not-found")]
        // public async Task<IActionResult> GetNotFound()
        // {
        //     var category = await unitOfWork.CategoryRepository.GetAllAsync(100);
        //     if(category == null) return NotFound();
        //     return Ok(category);
        // }
        // [HttpGet("not-found")]
        // public async Task<IActionResult> GetNotFound()
        // {
        //     var category = await unitOfWork.CategoryRepository.GetAllAsync(100);
        //     if(category == null) return NotFound();
        //     return Ok(category);
        // }


    }
}
