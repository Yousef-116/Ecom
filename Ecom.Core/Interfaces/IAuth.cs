using Ecom.Core.DTO;
using Ecom.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IAuth
    {

        Task<AuthResponse> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResponse> LoginAsync(LoginDTO loginDTO);
        Task SendEmail(string email, string Code, string component, string subject, string message);
        Task<bool> SendEmialForForgetPasswordAsync(string email);
        Task<AuthResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<bool> ActiveAccountAsync(ActiveAccountDTO activeAccount);


    }
}
