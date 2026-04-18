using AutoMapper;
using Ecom.API.Helper;
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

        // ========================= REGISTER =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await unitOfWork.Auth.RegisterAsync(dto);

            return result.Success
                ? StatusCode(201, new { Message = result.Message })
                : BadRequest(new { Message = result.Message });
        }

        // ========================= LOGIN =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            string token = await unitOfWork.Auth.LoginAsync(loginDTO);
            if (token.StartsWith("please"))
            {
                return BadRequest(new ResponseAPI(400, token));
            }

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Expires = DateTime.UtcNow.AddDays(1)
            });

            return Ok(new ResponseAPI(200, "Login successful"));
        }

        //[HttpPost("logout")]
        //public IActionResult Logout()
        //{
        //    Response.Cookies.Append("token", "", new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.None,
        //        IsEssential = true,
        //        Domain = "localhost",
        //        Expires = DateTime.Now.AddDays(-1)
        //    });
        //    return Ok(new { Message = "Logged out successfully" });
        //}
        // ========================= LOGOUT =========================
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");

            return NoContent();
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<IActionResult> GetAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var address = await unitOfWork.Auth.GetAddressAsync(email);
            var result = mapper.Map<ShippingAddressDTO>(address);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<IActionResult> UpdateAddress(ShippingAddressDTO addressDTO)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var address = mapper.Map<Address>(addressDTO);
            var result = await unitOfWork.Auth.UpdateAddress(email, address);

            return result ? Ok(new { Message = "Address updated successfully" }) : BadRequest(new { Message = "Address update failed" });
        }



        [Authorize]
        [HttpGet("user-name")]
        public IActionResult GetUserName()
        {
            var userName = User.Identity?.Name;
            return Ok(new { UserName = userName });
        }

        [HttpGet("is-auth")]
        public IActionResult IsUserAuth()
        {
            return User.Identity?.IsAuthenticated == true ? Ok() : Unauthorized();
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAccount([FromBody] ActiveAccountDTO accountDTO)
        {
            var result = await unitOfWork.Auth.ActiveAccountAsync(accountDTO);
            return result ? Ok("Activated successfully") : BadRequest("Activation failed");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await unitOfWork.Auth.SendEmialForForgetPasswordAsync(email);
            return result 
                ? Ok(new ResponseAPI(200, "Password reset email sent")) 
                : BadRequest(new ResponseAPI(400, "Failed to send reset email"));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO restPasswordDTO)
        {
            var result = await unitOfWork.Auth.ResetPasswordAsync(restPasswordDTO);
            return result.Success 
                ? Ok(new ResponseAPI(200, "Password reset successful")) 
                : BadRequest(new ResponseAPI(400, "Password reset failed"));
        }
    }
}

