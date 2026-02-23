using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public AuthRepository(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken generateToken)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.generateToken = generateToken;
        }

        //public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        //{
        //    if (registerDTO == null)
        //    {
        //        return null;
        //    }
        //    if (await userManager.FindByNameAsync(registerDTO.UserName) != null)
        //    {
        //        return "This UserName is Already Registerd";
        //    }
        //    if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
        //    {
        //        return "This Email is Already Registerd";
        //    }

        //    AppUser user = new AppUser
        //    {
        //        UserName = registerDTO.UserName,
        //        Email = registerDTO.Email,
        //        DisplayName = registerDTO.UserName,
        //        Address = new Address
        //        {
        //            FirstName = "N/A",
        //            LastName = "N/A",
        //            Street = "N/A",
        //            City = "N/A",
        //            State = "N/A",        // 🔥 THIS was missing
        //            ZipCode = "00000"
        //        }
        //    };

        //    try
        //    {


        //    var result = await userManager.CreateAsync(user, registerDTO.Password);

        //    if (!result.Succeeded)
        //    {
        //        return result.Errors.ToList()[0].Description;
        //    }

        //    // Send Active Email

        //    string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        //    await SendEmail(user.Email, token, "active", "Active Your Account", "Please Click The Link Below To Active Your Account");
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }


        //    return "Done";



        //}

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
                    DisplayName = registerDTO.UserName,
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

                await SendEmail(
                    user.Email,
                    token,
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

        //public async Task<string> LoginAsync(LoginDTO loginDTO)
        //{
        //    if (loginDTO == null)
        //    {
        //        return null;
        //    }

        //    var user = await userManager.FindByEmailAsync(loginDTO.Email);
        //    if (user == null)
        //    {
        //        return "Invalid UserName";
        //    }
        //    if (user.EmailConfirmed == false)
        //    {
        //        //return "Please Active Your Account First";
        //        string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        //        await SendEmail(user.Email, token, "active", "Active Your Account", "Please Click The Link Below To Active Your Account");
        //        return "Please Active Your Account First, We Have Send You An Email To Active Your Account";
        //    }

        //    var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, true);
        //    if (!result.Succeeded)
        //    {
        //        return "Invalid Password , please check your Email and password";
        //    }

        //    return generateToken.GetAndGenerateToken(user);

        //}

        public async Task<AuthResponse> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                if (loginDTO == null)
                    return AuthResponse.Fail("Invalid request");

                var user = await userManager.FindByEmailAsync(loginDTO.Email);

                if (user == null)
                    return AuthResponse.Fail("Invalid credentials");

                if (!user.EmailConfirmed)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    await SendEmail(
                        user.Email,
                        token,
                        "active",
                        "Activate Your Account",
                        "Please click the link below to activate your account"
                    );

                    return AuthResponse.Fail("Please confirm your email first. Activation email sent.");
                }

                var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, true);

                if (!result.Succeeded)
                    return AuthResponse.Fail("Invalid credentials");

                var jwtToken = generateToken.GetAndGenerateToken(user);

                return AuthResponse.Ok("Login successful", jwtToken);
            }
            catch (Exception ex)
            {
                return AuthResponse.Fail(ex.InnerException?.Message ?? ex.Message);
            }
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
            var user = await userManager.FindByEmailAsync(activeAccount.Email);
            if (user == null)
            {
                return false;
            }
            var result = await userManager.ConfirmEmailAsync(user, activeAccount.Token);
            if (!result.Succeeded)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendEmail(user.Email, token, "active", "Active Your Account", "Please Click The Link Below To Active Your Account");
                return false;
            }
            return true;

        }


    }

}
