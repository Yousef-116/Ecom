using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories
{
    internal class AuthRepository : IAuth
    {
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IGenerateToken generateToken;
        public AuthRepository(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken generateToken)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.generateToken = generateToken;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return null;
            }
            if (await userManager.FindByNameAsync(registerDTO.UserName) != null)
            {
                return "This UserName is Already Registerd";
            }
            if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
            {
                return "This Email is Already Registerd";
            }

            AppUser user = new AppUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email

            };

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return result.Errors.ToList()[0].Description;
            }

            // Send Active Email

            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendEmail(user.Email, token, "ActiveAccount", "Active Your Account", "Please Click The Link Below To Active Your Account");


            return "Done";



        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return null;
            }

            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return "Invalid UserName";
            }
            if (user.EmailConfirmed == false)
            {
                //return "Please Active Your Account First";
                string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendEmail(user.Email, token, "Reset-Password", "Active Your Account", "Please Click The Link Below To Active Your Account");
                return "Please Active Your Account First, We Have Send You An Email To Active Your Account";
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, true);
            if (!result.Succeeded)
            {
                return "Invalid Password , please check your Email and password";
            }

            return generateToken.GetAndGenerateToken(user);

        }

        public async Task SendEmail(string email, string Code, string component, string subject, string message)
        {
            var result = new EmailDTO(
              email, "yousef.com1162003@gmail.com",
              subject,
              EmailStringBody.
              send(email, Code, component, message));

            await emailService.SendEmail(result);
        }

        public async Task<bool> SendEmialForForgetPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await SendEmail(user.Email, token, "Reset-Password", "Reset Your Password", "Please Click The Link Below To Reset Your Password");

            return true;

        }
    }

}
