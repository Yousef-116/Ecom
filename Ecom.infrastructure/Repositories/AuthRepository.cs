using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
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
        private readonly UserManager<AppUser> userManager;
        public AuthRepository(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            if(registerDTO == null)
            {
                return null;
            }
            if(await userManager.FindByNameAsync(registerDTO.UserName) != null)
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

            if(!result.Succeeded)
            {
                return result.Errors.ToList()[0].Description;
            }

            // Send Active Email




            return "Done";



        }

    }
}
