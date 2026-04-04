using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecom.API.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            var result = await unitOfWork.Auth.RegisterAsync(registerDto);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);

        }

        [HttpGet("get-address-for-user")]
        public async Task<IActionResult> GetAddress()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);

            if (emailClaim == null)
                return Unauthorized();

            var address = await unitOfWork.Auth.GetAddressAsync(emailClaim.Value);

            var result = mapper.Map<ShippingAddressDTO>(address);

            return Ok(result);
        }

        [HttpGet("Logout")]
        public void logout()
        {

            Response.Cookies.Append("token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Domain = "localhost",
                Expires = DateTime.Now.AddDays(-1)
            });
        }

        [Authorize]
        [HttpGet("get-user-name")]
        public IActionResult GetUserName()
        {
            var userName = User.Identity.Name;
            return Ok(new
            {
                statusCode = 200,
                message = userName,
                data = userName,      // Add this
                userName = userName    // Add this
            });
        }

        [HttpGet("IsUserAuth")]
        public async Task<IActionResult> IsUserAuth()
        {
            return User.Identity.IsAuthenticated ? Ok() : Unauthorized();
        }


        [Authorize]
        [HttpPut("update-address")]
        public async Task<IActionResult> updateAddress(ShippingAddressDTO addressDTO)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var address = mapper.Map<Address>(addressDTO);
            var result = await unitOfWork.Auth.UpdateAddress(email, address);
            return result ? Ok() : BadRequest();
        }


        [HttpPost("Login")]
        public async Task<IActionResult> login(LoginDTO loginDTO)
        {

           
            string result = await unitOfWork.Auth.LoginAsync(loginDTO);
            if (result.StartsWith("please"))
            {
                return BadRequest(new ResponseAPI(400, result));
            }

            Response.Cookies.Append("token", result, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                    
                SameSite = SameSiteMode.None,    
                IsEssential = true,
                Expires = DateTime.UtcNow.AddDays(1)
            });
            return Ok(new ResponseAPI(200,"login successfully"));
        }


      
        [HttpPost("active-account")]
        public async Task<ActionResult> active([FromBody] ActiveAccountDTO accountDTO)
        {
           
            var result = await unitOfWork.Auth.ActiveAccountAsync(accountDTO);

            return result
                ? Ok("Activated successfully")
                : BadRequest("Activation failed");
        }


        [HttpGet("send-email-forget-password")]
        public async Task<IActionResult> forget(string email)
        {
            var result = await unitOfWork.Auth.SendEmialForForgetPasswordAsync(email);
            return result ? Ok(new ResponseAPI(200, "Email send successfully")) : BadRequest(new ResponseAPI(200, "email Not Activvvvvvvvvv"));
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> reset(ResetPasswordDTO restPasswordDTO)
        {
            var result = await unitOfWork.Auth.ResetPasswordAsync(restPasswordDTO);
            if (result.Success)
            {
                return Ok(new ResponseAPI(200, "login successfully"));
            }
            return BadRequest(new ResponseAPI(400, "Nottttttt reset Password "));
        }
    }
}

