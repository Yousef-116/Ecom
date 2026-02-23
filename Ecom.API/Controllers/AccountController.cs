using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            var result = await unitOfWork.Auth.RegisterAsync(registerDto);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var result = await unitOfWork.Auth.LoginAsync(loginDto);
            if (result.Success == false)
            {
                return BadRequest("Invalid UserName Or Password");
            }

            Response.Cookies.Append("token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Domain = "localhost", // Adjust this to your domain

                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Ok(result.Message);
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> ActiveAccount(ActiveAccountDTO activeAccount)
        {
            var result = await unitOfWork.Auth.ActiveAccountAsync(activeAccount);
            if (result)
            {
                return Ok("Account Activated Successfully");
            }
            return BadRequest("Invalid Activation Code");
        }
        [HttpPost("send-email-forget-password")]
        public async Task<IActionResult> SendEmailForForgetPassword(string email)
        {
            var result = await unitOfWork.Auth.SendEmialForForgetPasswordAsync(email);
            if (result)
            {
                return Ok("Email Sent Successfully");
            }
            return BadRequest("Failed to Send Email");
        }




    }
}
