using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize]
        [HttpPut("update-address")]
        public async Task<IActionResult> UpdateOrCreateAddress(ShippingAddressDTO shippingAddressDTO)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized("User not authenticated");

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
                return BadRequest("Email claim missing");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var address = mapper.Map<Address>(shippingAddressDTO);

            var result = await unitOfWork.Auth.UpdateAddress(email, address);

            if (result)
                return Ok(new { message = "Address Update Successfully" });


            return BadRequest("Failed to update address");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var result = await unitOfWork.Auth.LoginAsync(loginDto);
            if (result.Success == false)
            {
                return BadRequest("Invalid UserName Or Password");
            }

            //Response.Cookies.Append("token", result.Token, new CookieOptions
            //{
            //    //HttpOnly = true,
            //    Secure = true,
            //    Domain = "localhost", // Adjust this to your domain

            //    SameSite = SameSiteMode.None,
            //    Expires = DateTimeOffset.UtcNow.AddDays(3)
            //});

            //return Ok(result.Message);
            return Ok(new
            {
                message = result.Message,
                token = result.Token
            });


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
        [HttpGet("send-email-forget-password")]
        public async Task<IActionResult> SendEmailForForgetPassword(string email)
        {
            var result = await unitOfWork.Auth.SendEmialForForgetPasswordAsync(email);
            if (result)
            {
                return Ok("Email Sent Successfully");
            }
            return BadRequest("Failed to Send Email");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            var result = await unitOfWork.Auth.ResetPasswordAsync(resetPasswordDto);
            if (result.Success)
            {
                return Ok($"Password Reset Successfully {result.Message}");
            }
            return BadRequest($"Failed to Reset Password{result.Message}");
        }
    }
}

