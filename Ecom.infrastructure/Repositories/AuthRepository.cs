using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Ecom.infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
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
        private readonly AppDbContext context;
        public AuthRepository(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken generateToken, AppDbContext context)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.generateToken = generateToken;
            this.context = context;
        }


        public async Task<AuthResponse> RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                if (registerDTO == null)
                    return AuthResponse.Fail("Invalid request");

                if (await userManager.FindByNameAsync(registerDTO.UserName) != null)
                    return AuthResponse.Fail("Username already exists");

                if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
                    return AuthResponse.Fail("Email already exists");

                var user = new AppUser
                {
                    UserName = registerDTO.UserName,
                    Email = registerDTO.Email,
                    DisplayName = registerDTO.DisplayName,
                    Address = new Address
                    {
                        FirstName = "N/A",
                        LastName = "N/A",
                        Street = "N/A",
                        City = "N/A",
                        State = "N/A",
                        ZipCode = "00000"
                    }
                };

                var result = await userManager.CreateAsync(user, registerDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return AuthResponse.Fail(errors);
                }

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);

                await SendEmail(
                    user.Email,
                    encodedToken,
                    "active",
                    "Activate Your Account",
                    "Please click the link below to activate your account"
                );

                return AuthResponse.Ok("Registration successful. Please confirm your email.");
            }
            catch (Exception ex)
            {
                return AuthResponse.Fail(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<string> LoginAsync(LoginDTO login)
        {

            if (login == null)
            {
                return null;
            }
            var finduser = await userManager.FindByEmailAsync(login.Email);

            if (!finduser.EmailConfirmed)
            {
                string token = await userManager.GenerateEmailConfirmationTokenAsync(finduser);

                await SendEmail(finduser.Email, token, "active", "ActiveEmail", "Please active your email, click on button to active");

                return "Please confirem your email first, we have send activat to your E-mail";
            }

            var result = await signInManager.CheckPasswordSignInAsync(finduser, login.Password, true);

            if (result.Succeeded)
            {
                return generateToken.GetAndGenerateToken(finduser);
            }

            return "please check your email and password, something went wrong";
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

        //public async Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        //{
        //    var user = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
        //    if (user == null)
        //    {
        //        return null;
        //    }
        //    var result = await userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.Password);
        //    if (!result.Succeeded)
        //    {
        //        return result.Errors.ToList()[0].Description;
        //    }
        //    return "Done , Password change success";
        //}

        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                    return AuthResponse.Fail("User not found");

                var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return AuthResponse.Fail(errors);
                }

                return AuthResponse.Ok("Password changed successfully");
            }
            catch (Exception ex)
            {
                return AuthResponse.Fail(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<bool> ActiveAccountAsync(ActiveAccountDTO activeAccount)
        {
            var user = await userManager.FindByEmailAsync(activeAccount.email);
            if (user == null)
            {
                return false;
            }

            var result = await userManager.ConfirmEmailAsync(user, activeAccount.token);

            if (!result.Succeeded)
            {
                
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"Confirmation failed: {errors}");

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendEmail(user.Email, token, "active", "Active Your Account", "Please Click The Link Below To Active Your Account");
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateAddress(string email, Address newAddress)
        {
            var user = await context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return false;

            if (user.Address == null)
            {
                newAddress.AppUserId = user.Id;
                user.Address = newAddress;
            }
            else
            {
                user.Address.FirstName = newAddress.FirstName;
                user.Address.LastName = newAddress.LastName;
                user.Address.Street = newAddress.Street;
                user.Address.City = newAddress.City;
                user.Address.State = newAddress.State;
                user.Address.ZipCode = newAddress.ZipCode;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Address> GetAddressAsync(string email)
        {
            //throw new NotImplementedException();
            var user = await context.Users
               .Include(u => u.Address)
               .FirstOrDefaultAsync(x => x.Email == email);

            return user.Address;


        }
    }

}
